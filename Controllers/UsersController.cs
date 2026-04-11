using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Library_Business;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Gets all user's info, provided by their id.
        /// </summary>
        /// <param name="userID">User ID.</param>
        /// <param name="Password">New password to set.</param>
        /// <returns>An object full of all user's info.</returns>
        [HttpGet("{userID}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<clsUserDTO> GetUserByID(int userID)
        {
            if (userID < 0)
                return BadRequest("Id is not valid");

            clsUser user = clsUser.Find(userID);

            if (user == null)
                return NotFound($"User with id {userID} is not found.");

            return Ok(user.userDTO);
        }

        /// <summary>
        /// Gets all user's info, provided by their username and password.
        /// </summary>
        /// <param name="Username">User's username</param>
        /// <param name="Password">User's passwrd</param>
        /// <returns>An object full of all user's info.</returns>
        [HttpGet("{Username}/{Password}", Name = "GetUserByUsernameAndPassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<clsUserDTO> GetUserByUsernameAndPassword(string Username, string Password)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                return BadRequest("Input is invalid");

            clsUser user = clsUser.Find(Username, Password);

            if (user == null)
                return NotFound($"User with username {Username} and password {Password} is not found");

            return Ok(user.userDTO);
        }

        /// <summary>
        /// Updates a user's password, provided by their userID.
        /// </summary>
        /// <param name="UserID">User ID</param>
        [HttpPut("{UserID}/{Password}", Name = "UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdatePassword(int UserID, string Password)
        {
            if (UserID < 0 || string.IsNullOrEmpty(Password))
                return BadRequest("Input is invalid");

            if (clsUser.Find(UserID) == null)
                return NotFound($"User with id {UserID} is not found");

            bool result = clsUser.UpdatePassword(UserID, Password);

            if (result)
                return Ok("Password is updated suceesfully");

            return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while updating the password." });
        }

        /// <summary>
        /// Gets all users with their important info (UserID, Username, Role, IsActive).
        /// </summary>
        /// <returns>A list of users with their important info.</returns>
        [HttpGet("All", Name = "GetAllUsersAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsUserDTOImportantFields>>> GetAllUsersAsync()
        {
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
        [HttpPost(Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<clsUserDTO> AddNewUser(clsUserDTO addedUser)
        {
            if (clsUser.IsValidInput(addedUser))
                return BadRequest("Input is invalid");

            clsUser user = new clsUser(new clsUserDTO(addedUser.UserID, addedUser.Username,
                addedUser.Password, addedUser.Role, addedUser.IsActive, addedUser.Permissions));

             if(!user.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new user." });

            addedUser.UserID = user.UserID;
            addedUser.Password = clsUtil.ComputeHash(user.Password);
            return CreatedAtRoute("GetUserByID", new { userID = user.UserID }, user.userDTO);
        }

        /// <summary>
        /// Determines whether the given username is used before in the system.
        /// </summary>
        /// <param name="username">Username to check if used before.</param>
        /// <returns>Whether the specified username is used before.</returns>
        [HttpGet("DoesUsernameExist/{username}", Name = "DoesUsernameExist")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> DoesUsernameExist(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username should be provided");

            return Ok(clsUser.DoesUsernameExist(username));
        }

        /// <summary>
        /// Determines whether the given password is used before by a specific user.
        /// </summary>
        /// <param name="UserID">The userID of the user to check for.</param>
        /// <param name="Password">The password to check if used before by the specified user.</param>
        /// <returns>Whether the specified password is used before by the specified user.</returns>
        [HttpGet("IsPasswordUsedByUser/{UserID}/{Password}", Name = "IsPasswordUsedByUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> IsPasswordUsedByUser(int UserID, string Password)
        {
            if (string.IsNullOrEmpty(Password) || UserID < 0)
                return BadRequest("Password should be provided");

            return Ok(clsUser.IsPasswordUsedByUser(UserID, Password));
        }
    }
}