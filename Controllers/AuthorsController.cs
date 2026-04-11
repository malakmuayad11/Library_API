using Library_Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        /// <summary>
        /// Get all author's info, provided by a book's id.
        /// </summary>
        /// <param name="BookID">The ID of the book the author has written.</param>
        /// <returns>An object full of all author's info.</returns>
        [HttpGet("{BookID}", Name = "GetByBookID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsAuthorDTO> GetByBookID(int BookID)
        {
            if (BookID < 0)
                return BadRequest("ID is invalid");
               
            clsAuthor author = clsAuthor.FindByBookID(BookID);

            if (author != null)
                return Ok(author.authorDTO);

            return NotFound($"Author with book id {BookID} is not found");
        }

        /// <summary>
        /// Get all author's info, provided by their first and last name.
        /// </summary>
        /// <param name="FirstName">The first name of the book to find.</param>
        /// <param name="LastName">The last name of the book to find.</param>
        /// <returns>An object full of all author's info.</returns>
        [HttpGet("{FirstName}/{LastName}", Name = "Find")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsAuthorDTO> Find(string FirstName, string LastName)
        {
            if(string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                    return BadRequest("Input is invalid");

            clsAuthor author = clsAuthor.Find(FirstName, LastName);

            if (author != null)
                return Ok(author.authorDTO);

            return NotFound($"Author with first name {FirstName} and last name {LastName} is not found");
        }

        /// <summary>
        /// Checks wether an author exists in the system, by their first and last name.
        /// </summary>
        /// <param name="FirstName">The first name of the author to check for.</param>
        /// <param name="LastName">The last name of the author to check for.</param>
        /// <returns>Whether the author exists in the system or not.</returns>
        [HttpGet("IsAuthorExists/{FirstName}/{LastName}", Name = "IsAuthorExists")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> IsAuthorExists(string FirstName, string LastName) =>
            (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName)) ?
            BadRequest("Input is invalid") : Ok(clsAuthor.IsAuthorExists(FirstName, LastName));
    }
}