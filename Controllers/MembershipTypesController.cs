using Library_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Models.DTOs;

namespace Library_System_API.Controllers
{
    [Authorize]
    [Route("api/Library/MembershipTypes")]
    [ApiController]
    public class MembershipTypesController : ControllerBase
    {
        /// <summary>
        /// Gets all membership types with their info.
        /// </summary>
        /// <returns>A list of membership tyes.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("All", Name = "GetAllMembershipTypesAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsMembershipTypeDTO>>> GetAllMembershipTypesAsync()
        {
            List<clsMembershipTypeDTO> membershipTypes = await clsMembershipType.GetAllMembershipTypesAsync();

            if (membershipTypes == null || membershipTypes.Count == 0)
                    return NotFound("Membership types are not found");

            return Ok(membershipTypes);
        }

        /// <summary>
        /// Gets all membership type's info, provided by its id.
        /// </summary>
        /// <param name="MembershipTypeID">Membership Type ID.</param>
        /// <returns>An object full of all membership type's info.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("{MembershipTypeID}", Name = "GetMembershipTypeByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<clsMembershipTypeDTO> GetMembershipTypeByID(int MembershipTypeID)
        {
            if (MembershipTypeID < 0)
                return BadRequest("Id is not valid");

            clsMembershipType membershipType = clsMembershipType.Find(MembershipTypeID);

            if (membershipType == null)
                return NotFound($"Membership type with id {MembershipTypeID} is not found.");

            return Ok(membershipType.membershipTypeDTO);
        }
    }
}