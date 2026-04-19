using Infrastructure.Logging;
using Library_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Models.DTOs;
using System.Security.Claims;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IConfiguration _Configuration;
        private readonly clsLoggerService _Logger; 
        public UsersController(IAuthorizationService authorizationService, IConfiguration Configuration, 
            clsLoggerService Logger)
        {
            this.authorizationService = authorizationService;
            this._Configuration = Configuration;
            this._Logger = Logger;
        }

        /// <summary>
        /// Gets all user's info, provided by their id.
        /// </summary>
        /// <param name="userID">User ID.</param>
        /// <returns>An object full of all user's info.</returns>
        [Authorize]
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("{userID}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<clsUserDTO>> GetUserByID(int userID)
        {
            if (userID < 0)
                return BadRequest("Id is not valid");

            clsUser user = clsUser.Find(userID);

            if (user == null)
                return NotFound($"User with id {userID} is not found.");

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                userID,
                "UserOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            return Ok(user.userDTO);
        }

        /// <summary>
        /// Gets all user's info, provided by their username and password.
        /// </summary>
        /// <param name="Username">User's username</param>
        /// <param name="Password">User's passwrd</param>
        /// <returns>An object full of all user's info.</returns>
        [Authorize(Policy = "ManageUsers")]
        [EnableRateLimiting("AuthLimiter")]
        [HttpPost("{Username}", Name = "GetUserByUsernameAndPassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public ActionResult<clsUserDTO> GetUserByUsernameAndPassword(string Username, clsPasswordRequestDTO passwordDTO)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(passwordDTO.Password))
                return BadRequest("Input is invalid");

            clsUser user = clsUser.Find(Username, passwordDTO.Password);

            if (user == null)
                return NotFound($"User with username {Username} and password {passwordDTO.Password} is not found");

            return Ok(user.userDTO);
        }

        /// <summary>
        /// Updates a user's password, provided by their userID.
        /// </summary>
        /// <param name="UserID">User ID</param>
        [Authorize]
        [EnableRateLimiting("AuthLimiter")]
        [HttpPut("{UserID}", Name = "UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult> UpdatePassword(int UserID, clsPasswordRequestDTO passwordDTO)
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (UserID < 0 || string.IsNullOrEmpty(passwordDTO.Password))
                return BadRequest("Input is invalid");

            if (clsUser.Find(UserID) == null)
                return NotFound($"User with id {UserID} is not found");

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                UserID,
                "UserOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            bool result = clsUser.UpdatePassword(UserID, passwordDTO.Password);

            if (result)
            {
                _Logger.Log(ip, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown",
                    "Password upated.");
                return Ok("Password is updated suceesfully");
            }

            return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while updating the password." });
        }

        /// <summary>
        /// Gets all users with their important info (UserID, Username, Role, IsActive).
        /// </summary>
        /// <returns>A list of users with their important info.</returns>
        [Authorize(Policy = "ManageUsers")]
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("All", Name = "GetAllUsersAsync")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsUserDTOImportantFields>>> GetAllUsersAsync()
        {
            Console.WriteLine(User.Identity?.IsAuthenticated);
            Console.WriteLine(User.Identity?.Name);

            List<clsUserDTOImportantFields> users = await clsUser.GetAllUsersAsync();

            if (users == null || users.Count == 0)
                return NotFound("Users are not found");

            return Ok(users);
        }

        /// <summary>
        /// Adds new user.
        /// </summary>
        /// <param name="addedUser">User's info to be added.</param>
        /// <returns>An object full of all the added user's info.</returns>
        [Authorize(Policy = "ManageUsers")]
        [EnableRateLimiting("CriticalOpsLimiter")]
        [HttpPost(Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<clsUserDTO> AddNewUser(clsUserDTO addedUser)
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            if (!clsUser.IsValidInput(addedUser))
                return BadRequest("Input is invalid");

            clsUser user = new clsUser(new clsUserDTO(addedUser.UserID, addedUser.Username,
                addedUser.Password, addedUser.Role, addedUser.IsActive, addedUser.Permissions));

             if(!user.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new user." });

            addedUser.UserID = user.UserID;
            addedUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _Logger.Log(ip, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown",
                    $"Created new User with ID {addedUser.UserID}.");
            return CreatedAtRoute("GetUserByID", new { userID = user.UserID }, user.userDTO);
        }

        /// <summary>
        /// Determines whether the given username is used before in the system.
        /// </summary>
        /// <param name="username">Username to check if used before.</param>
        /// <returns>Whether the specified username is used before.</returns>
        [Authorize(Policy = "ManageUsers")]
        [EnableRateLimiting("AuthLimiter")]
        [HttpGet("DoesUsernameExist/{username}", Name = "DoesUsernameExist")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public ActionResult<bool> DoesUsernameExist(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username should be provided");

            return Ok(clsUser.DoesUsernameExist(username));
        }
    }
}