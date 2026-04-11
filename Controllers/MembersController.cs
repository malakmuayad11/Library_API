using Library_Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Models;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        /// <summary>
        /// Adds new member.
        /// </summary>
        /// <param name="addedMember">Member's info to be added.</param>
        /// <returns>An object full of all the added member's info if input is valid
        /// and no server error occurs.</returns>
        [HttpPost(Name = "AddNewMember")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddNewMember(clsMemberDTO addedMember)
        {
            if (!clsMember.IsValidMemberInput(addedMember))
                return BadRequest("Input is invalid");

            clsMember newMember = new clsMember(new clsMemberDTO(addedMember.MemberID, addedMember.FirstName,
                addedMember.SecondName, addedMember.ThirdName, addedMember.LastName, addedMember.DateOfBirth,
                addedMember.Address, addedMember.Phone, addedMember.Email, addedMember.ImagePath,
                addedMember.MembershipTypeID, addedMember.StartDate, addedMember.ExpiryDate, addedMember.IsCancelled));

            if(!newMember.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new member." });

            addedMember.MemberID = newMember.MemberID;

            return CreatedAtRoute("GetMemberByMemberID", new { MemberID = addedMember.MemberID }, newMember.memberDTO);
        }

        /// <summary>
        /// Updates all member's info with provided data. Member ID cannot be updated.
        /// </summary>
        /// <param name="MemberID">The ID of the member that will be updated.</param>
        /// <param name="updatedMemberDTO">New info of that member.</param>
        /// <returns>An object full of the updated member's info, if the input
        /// is valid, and no server error occurs.</returns>
        [HttpPut("{MemberID}", Name = "UpdateMember")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<clsMemberDTO> UpdateMember(int MemberID, clsMemberDTO updatedMemberDTO)
        {
            if (!clsMember.IsValidMemberInput(updatedMemberDTO))
                return BadRequest("Input is invalid");

            clsMember member = clsMember.Find(MemberID);
            if (member == null)
                return NotFound($"Member with id {MemberID} is not found");

            member.FirstName = updatedMemberDTO.FirstName;
            member.SecondName = updatedMemberDTO.SecondName;
            member.ThirdName = updatedMemberDTO.ThirdName;
            member.LastName = updatedMemberDTO.LastName;
            member.DateOfBirth = updatedMemberDTO.DateOfBirth;
            member.Address = updatedMemberDTO.Address;
            member.Phone = updatedMemberDTO.Phone;
            member.Email = updatedMemberDTO.Email;
            member.ImagePath = updatedMemberDTO.ImagePath;
            member.MembershipTypeID = updatedMemberDTO.MembershipTypeID;
            member.StartDate = updatedMemberDTO.StartDate;
            member.ExpiryDate = updatedMemberDTO.ExpiryDate;
            member.IsCancelled = updatedMemberDTO.IsCancelled;

            if (member.Save())
                return Ok(member.memberDTO);

            return StatusCode(500,new { message = "An error occurred while updating the member" });
        }

        /// <summary>
        /// Get all member's info, provided by their id.
        /// </summary>
        /// <param name="MemberID">The ID of the member to find.</param>
        /// <returns>An object full of all member's info.</returns>
        [HttpGet("{MemberID}", Name = "GetMemberByMemberID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<clsMemberDTO> GetMemberByMemberID(int MemberID)
        {
            if (MemberID < 0)
                return BadRequest("Input is invalid");

            clsMember member = clsMember.Find(MemberID);

            if (member == null)
                return NotFound($"Member with id {MemberID} is not found");

            return Ok(member.memberDTO);
        }

        /// <summary>
        /// Updates the status (whether cancelled or not) of the membership.
        /// </summary>
        /// <param name="MemberID">The ID of the membership to be updated.</param>
        /// <param name="IsCancelled">The new status of that member.</param>
        /// <returns>Whether the membership's status is updated sucessfully.</returns>
        [HttpPatch("{MemberID}/{IsCancelled}", Name = "UpdateCancel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> UpdateCancel(int MemberID, bool IsCancelled) =>
            (MemberID < 0) ? (BadRequest("Input is invalid")) : (Ok(clsMember.UpdateCancel(MemberID, IsCancelled)));

        /// <summary>
        /// Gets all members with their approptiate info to display in the presentation layer.
        /// </summary>
        /// <returns>A list of members with their appropriate info.</returns>
        [HttpGet("All", Name = "GetAllMembersAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsMemberGetAllDTO>>> GetAllMembersAsync()
        {
            List<clsMemberGetAllDTO> members = await clsMember.GetAllMembersAsync();

            if (members == null || members.Count == 0)
                return NotFound("Members are not found");

            return Ok(members);
        }


        /// <summary>
        /// Renews the membership of the specified member.
        /// </summary>
        /// <param name="MemberID">The id of the membership to renew.</param>
        /// <returns>Whether the membership is renewed successfully or not.</returns>
        [HttpPatch("RenewMembership/{MemberID}", Name = "RenewMembership")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> RenewMembership(int MemberID) =>
            (MemberID < 0) ? (BadRequest("Input is invalid")) : (Ok(clsMember.RenewMembership(MemberID)));


        /// <summary>
        /// Returns the number of borrowed books by a specific member.
        /// </summary>
        /// <param name="MemberID">The id of the member to get their borrowed books.</param>
        /// <returns>The number of borrowed book by the member.</returns>
        [HttpGet("GetNumberOfBorrowedBook/{MemberID}", Name = "GetNumberOfBorrowedBook")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<int> GetNumberOfBorrowedBook(int MemberID)
        {
            if (MemberID < 0)
                return BadRequest("Invalid input");

            int numOfBorrowedBook = clsMember.GetNumberOfBorrowedBook(MemberID);

            if (numOfBorrowedBook == -1)
                return NotFound("Number of borrowed books is not found");

            return Ok(numOfBorrowedBook);
        }
    }
}