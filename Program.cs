using Azure.Identity;
using Infrastructure.Logging;
using Library_Business;
using Library_Data;
using Library_System_API.Authorization.Handlers;
using Library_System_API.Authorization.Requirements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Load Azure Key Vault
var keyVaultUrl = builder.Configuration["KeyVault:Url"];

if (!string.IsNullOrWhiteSpace(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

//🔹 Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
    options.AddPolicy("CriticalOpsLimiter", httpContext =>
    {
        string userID =
            httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userID,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
    options.AddPolicy("LightOpsLimiter", httpContext =>
    {
        string userID =
            httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userID,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

// 🔹 Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<Infrastructure.Logging.ILogger, clsNotepadLogger>();
builder.Services.AddScoped<clsLoggerService>();

// 🔹 Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    // Define JWT Bearer scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // Require JWT in Swagger
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("LibrarySystemApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 🔹 JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSigningKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new Exception("JWT Signing Key not found in Azure Key Vault.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "LibrarySystemApi",
            ValidAudience = "LibrarySystemApiUsers",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            ),
            RoleClaimType = ClaimTypes.Role
        };
    });

// 🔹 Adding Policies
builder.Services.AddSingleton<IAuthorizationHandler, UserOwnerOrAdminHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, HasUserPermissionsHandler>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOwnerOrAdmin", policy =>
        policy.Requirements.Add(new UserOwnerOrAdminRequirement()));

    options.AddPolicy("ManageMembers", policy =>
        policy.Requirements.Add(new HasUserPermissionsRequirement((int)clsUser.enPermissions.eManageMembers)));

    options.AddPolicy("ManageBooks", policy =>
        policy.Requirements.Add(new HasUserPermissionsRequirement((int)clsUser.enPermissions.eManageBooks)));

    options.AddPolicy("ManageCourses", policy =>
        policy.Requirements.Add(new HasUserPermissionsRequirement((int)clsUser.enPermissions.eManageCourses)));

    options.AddPolicy("ManageUsers", policy =>
        policy.Requirements.Add(new HasUserPermissionsRequirement((int)clsUser.enPermissions.eManageUsers)));

    options.AddPolicy("ManagePayments", policy =>
        policy.Requirements.Add(new HasUserPermissionsRequirement((int)clsUser.enPermissions.eManagePayments)));
});

var app = builder.Build();

clsSettingsData.Initialize(builder.Configuration);
//clsLoggerData.Logger = app.Services.GetRequiredService<clsLoggerService>();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("LibrarySystemApiCorsPolicy");

app.UseRateLimiter();
//Safe Message for rate limiter
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        await context.Response.WriteAsync("Too many attempts. Please try again later.");
    }
});

// Logging Forbidden Access
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        var loggerService = context.RequestServices.GetRequiredService<clsLoggerService>();

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        int userId = int.TryParse(userIdClaim, out var id) ? id : 0;

        loggerService.Log(ip, userIdClaim, "Forbidden Access");
    }
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();