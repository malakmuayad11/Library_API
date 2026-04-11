using Library_Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Fines")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        /// <summary>
        /// Adds a new fine to a loan that is not returned by its due date.
        /// </summary>
        /// <param name="addedFine">Fine's info to be added.</param>
        /// <returns>An object full of all the added fine's info if input is valid
        /// and no server error occurs.</returns>
        [HttpPost(Name = "AddNewFine")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<clsFineDTO> AddNewFine(clsFineDTO addedFine)
        {
            if (!clsFine.IsValidInput(addedFine))
                return BadRequest("Input is invalid");

            clsFine newFine = new clsFine(new clsFineDTO(addedFine.FineID, addedFine.MemberID,
                addedFine.LoanID, addedFine.FineAmount, addedFine.IsPaid));

            if(!newFine.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new fine." });

            addedFine.FineID = newFine.FineID;

            return Ok(newFine.fineDTO);
        }

        /// <summary>
        /// Gets all fines with all their info.
        /// </summary
        /// <returns>A list of fines with their info.</returns>
        [HttpGet("All", Name = "GetAllFinesAsync")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsFineDTO>>> GetAllFinesAsync()
        {
            List<clsFineDTO> fines = await clsFine.GetAllFinesAsync();

            if (fines.Count < 0 || fines == null)
                return BadRequest("Input is invalid");

            return Ok(fines);
        }

        /// <summary>
        /// Updates the payment status (paid or not paid) for a fine.
        /// </summary>
        /// <param name="FineID">The ID of the fine to update its payment status.</param>
        /// <param name="IsPaid">Whether to update the status to paid or not paid.</param>
        /// <returns>Whether the payment status is paid successfully or not.</returns>
        [HttpPatch("PaymentStatus/{FineID}/{IsPaid}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> UpdatePaymentStatus(int FineID, bool IsPaid) =>
            (FineID < 0) ? BadRequest("Input is invalid") : Ok(clsFine.UpdatePaymentStatus(FineID, IsPaid));

        /// <summary>
        /// Pays the fine amount of a specific fine.
        /// </summary>
        /// <param name="FineID">The fine to pay its fine amount.</param>
        /// <returns>Whether the fine amount is updated successfully or not.</returns>
        [HttpPatch("Pay/{FineID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> PayFines(int FineID) =>
            (FineID < 0) ? BadRequest("Input is invalid") : Ok(clsFine.PayFines(FineID));


        /// <summary>
        /// Gets the amount of the unpaid fine amount for a specific member.
        /// </summary>
        /// <param name="MemberID">The ID of the member to get their unpaid fees.</param>
        /// <returns>The number of the unpaid fees for the member. If there is no unpaid fees, 
        /// the method will return -1.</returns>
        [HttpGet("UnpaidFees/{MemberID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<decimal> GetMemberUnpaidFees(int MemberID) =>
            (MemberID < 0) ? BadRequest("Input is invalid") : Ok(clsFine.GetMemberUnpaidFees(MemberID) ?? 0);
    }
}