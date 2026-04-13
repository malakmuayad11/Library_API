using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Load Azure Key Vault
var keyVaultUrl = builder.Configuration["KeyVault:Url"];

if (!string.IsNullOrWhiteSpace(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

// 🔹 Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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
            )
        };
    });

var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("LibrarySystemApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();