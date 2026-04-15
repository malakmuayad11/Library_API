using Library_Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Library_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Configuration;

        public AuthController(IConfiguration configuration) => _Configuration = configuration;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            clsUser user = clsUser.Find(request.Username);

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (!user.IsActive)
                return Unauthorized("This account is not active.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            if(token == null)
                return StatusCode(500, "JWT key missing from Key Vault");

            string refreshToken = GenerateRefreshToken();

            if (!clsUsersToken.Login(user.UserID, BCrypt.Net.BCrypt.HashPassword(refreshToken),
                DateTime.UtcNow.AddDays(7)))
                return StatusCode(500, "An error occurred while logging in");

            return Ok(new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            clsUser user = clsUser.Find(request.Username);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid refresh request");

            var(expiresAt, revokedAt, hash)
                = clsUsersToken.GetTokenDataForUser(user.UserID);

            if (revokedAt != null)
                return Unauthorized("Refresh token is revoked");

            if (expiresAt == null || expiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expired");

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, hash);
            if (!refreshValid)
                return Unauthorized("Invalid refresh token");

            var token = GenerateJwtToken(user);
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Rotation: replace refresh token
            var newRefreshToken = GenerateRefreshToken();
            string NewRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            
            if (!clsUsersToken.Refresh(user.UserID, NewRefreshTokenHash, DateTime.UtcNow.AddDays(7)))
                return StatusCode(500, "An error occurred during refresh");

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            clsUser user = clsUser.Find(request.Username);

            if (user == null)
                return Ok(); // Do not reveal if user exists

            if (!BCrypt.Net.BCrypt.Verify(request.RefreshToken,
                clsUsersToken.GetRefreshTokenHashForUser(user.UserID)))
                return Ok();

            if (!clsUsersToken.Logout(user.UserID, DateTime.UtcNow))
                return StatusCode(500, "An error occurred while logging out");

            return Ok("Logged out successfully");
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private JwtSecurityToken GenerateJwtToken(clsUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.RoleString),
                new Claim("permissions", user.PermissionsInt.ToString())
            };

            var secretKey = _Configuration["JwtSigningKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
                return null;

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: "LibrarySystemApi",
                audience: "LibrarySystemApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
        }
    }
}