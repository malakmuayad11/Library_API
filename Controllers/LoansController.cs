using Infrastructure.Logging;
using Library_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Models.DTOs;
using System.Security.Claims;

namespace Library_System_API.Controllers
{
    [Authorize(Policy = "ManagePayments")]
    [Route("api/Library/Loans")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly clsLoggerService _Logger;

        public LoansController(IConfiguration configuration, clsLoggerService logger)
        {
            _Configuration = configuration;
            _Logger = logger;
        }

        /// <summary>
        /// Gets all loans with their approptiate info to display in the presentation layer.
        /// </summary
        /// <returns>A list of loans with their appropriate info.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet(Name = "GetAllLoans")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsLoanGetAllDTO>>> GetAllLoansAsync()
        {
            List<clsLoanGetAllDTO> loans = await clsLoan.GetAllLoansAsync();

            if (loans.Count == 0 || loans == null)
                return NotFound("Loans are not found");

            return Ok(loans);
        }

        /// <summary>
        /// Adds a new loan.
        /// </summary>
        /// <param name="BookID">The ID of the book to add in the loan.</param>
        /// <param name="MemberID">The ID of the member to add in the loan.</param>
        /// <param name="CreatedByUserID">The ID of the user who created the loan.</param>
        /// <returns>An object full of all the added loan's info if input is valid
        /// and no server error occurs.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpPost(Name = "AddNewLoan")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult AddNewLoan(int BookID, int MemberID, int CreatedByUserID)
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (BookID < 0 || MemberID < 0 || CreatedByUserID < 0)
                return BadRequest("Input is invalid");

            clsLoan newLoan = new clsLoan(new clsLoanDTO(-1, BookID, MemberID,
                DateTime.Now, DateTime.Now, null, null, CreatedByUserID));

            if (!newLoan.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new loan." });

            _Logger.Log(ip, CreatedByUserID.ToString(), $"Added new loan, with loanID: {newLoan.LoanID}.");
            return CreatedAtRoute("GetLoanByID", new { LoanID = newLoan.LoanID }, newLoan.loanDTO);
        }

        /// <summary>
        /// Gets all loan's info, provided by its id.
        /// </summary>
        /// <param name="LoanID">The ID of the loan to find.</param>
        /// <returns>An object full of all loan's info.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("{LoanID}", Name = "GetLoanByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsLoanDTO> GetLoanByID(int LoanID)
        {
            if (LoanID < 0)
                return BadRequest("Input is not valid");

            clsLoan loan = clsLoan.Find(LoanID);

            if (loan != null)
                return Ok(loan.loanDTO);

            return NotFound($"Loan with id {LoanID} is not found");
        }

        /// <summary>
        /// Gets all loans's info, provided by a member's id.
        /// </summary>
        /// <param name="MemberID">The ID of the member who has the loan.</param>
        /// <returns>An object full of all loan's info.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("Loan/{MemberID}", Name = "GetLoanByMemberID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsLoanDTO> GetLoanByMemberID(int MemberID)
        {
            if (MemberID < 0)
                return BadRequest("Input is invalid");

            clsLoan loan = clsLoan.FindByMemberID(MemberID);

            if (loan == null)
                return NotFound($"Loan with memberID {MemberID} is not found");

            return Ok(loan.loanDTO);
        }

        /// <summary>
        /// This method Returns a book (loan) in the system.
        /// </summary>
        /// <param name="LoanID">The id of the loan to return in the system.</param>
        /// <returns>Whether the loan is returned successfully or not.</returns>
        [EnableRateLimiting("CriticalOpsLimiter")]
        [HttpPatch("Return/{LoanID}", Name = "Return")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> Return(int LoanID)
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (LoanID < 0)
                return BadRequest("Input is invalid");

            clsLoan loan = clsLoan.Find(LoanID);

            if (loan == null)
                return NotFound($"Loan with id {LoanID} is not found");

            _Logger.Log(ip, userID, $"returned loan, with loanID: {loan.LoanID}.");
            return Ok(loan.Return(LoanID));
        }

        /// <summary>
        /// Whether a member can return a book (loan).
        /// </summary>
        /// <param name="LoanID">The ID of the loan that can be returned.</param>
        /// <returns>Whether a loan can be returned.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("CanReturnBook/{LoanID}", Name = "CanReturnBook")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> CanReturnBook(int LoanID) =>
            (LoanID < 0) ? BadRequest("Input is invalid") : Ok(clsLoan.CanReturnBook(LoanID));

        /// <summary>
        /// Whether a member can extend a loan.
        /// </summary>
        /// <param name="LoanID">The ID of the loan that can be extended.</param>
        /// <returns>Whether the loan can be extended.</returns>
        [EnableRateLimiting("LightOpsLimiter")]
        [HttpGet("CanExtendLoan/{LoanID}", Name = "CanExtendLoan")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> CanExtendLoan(int LoanID) =>
            (LoanID < 0) ? BadRequest("Input is invalid") : Ok(clsLoan.CanExtendLoan(LoanID));

        /// <summary>
        /// Extends a loan, by extending its due date.
        /// </summary>
        /// <param name="LoanID">The ID of the loan to be extended.</param>
        /// <param name="DueDate">The new due date of the extended loan.</param>
        /// <returns>Whether the loan is extended successfully or not.</returns>
        [EnableRateLimiting("CriticalOpsLimiter")]
        [HttpPatch("ExtendDueDate/{LoanID}/{DueDate}", Name = "ExtendDueDate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> ExtendDueDate(int LoanID, DateTime DueDate) 
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (LoanID < 0)
                return BadRequest("Input is invalid");

            _Logger.Log(ip, userID, $"Extended due date of loan with loadID: {LoanID}.");
            return Ok(clsLoan.ExtendDueDate(LoanID, DueDate));
        }
    }
}