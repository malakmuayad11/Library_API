using Library_Business;
using Microsoft.AspNetCore.Mvc;
using Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.RateLimiting;

namespace Library_System_API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly Infrastructure.Logging.ILogger _Logger;

        public AuthController(IConfiguration configuration, Infrastructure.Logging.ILogger Logger)
        {
            _Configuration = configuration;
            _Logger = Logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            clsUser user = clsUser.Find(request.Username);

            if (user == null)
            {
                _Logger.Log(ip, "Unknown", "Failed Login");
                return Unauthorized("Invalid credentials");
            }

            if (!user.IsActive)
            {
                _Logger.Log(ip, user.UserID.ToString(), "Login attempt failed: inactive account");
                return Unauthorized("This account is not active.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                _Logger.Log(ip, user.UserID.ToString(),
                    "Failed login attempt (bad password).");
                return Unauthorized("Invalid credentials");
            }

            var token = clsTokenService.GenerateJwtToken(user, _Configuration);
            if(token == null)
                return StatusCode(500, "JWT key missing from Key Vault");

            string refreshToken = clsTokenService.GenerateRefreshToken();

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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
            {
                _Logger.Log(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    user.UserID.ToString(), "Invalid refresh token attempt.");
                return Unauthorized("Invalid refresh token");
            }

            var token = clsTokenService.GenerateJwtToken(user, _Configuration);
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Rotation: replace refresh token
            var newRefreshToken = clsTokenService.GenerateRefreshToken();
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

        
    }
}