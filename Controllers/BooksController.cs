using Library_Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        /// <summary>
        /// Gets all book's info, provided by its id.
        /// </summary>
        /// <param name="BookID">The ID of the book to find.</param>
        /// <returns>An object full of all book's info.</returns>
        [HttpGet("{BookID}", Name = "FindBookByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsBookDTO> FindBookByID(int BookID)
        {
            if (BookID < 0)
                return BadRequest("Input is invalid");

            clsBook book = clsBook.Find(BookID);

            if(book != null)
                return Ok(book.bookDTO);

            return NotFound($"Book with id {BookID} is not found");
        }

        /// <summary>
        /// Get all book's info, provided by its title.
        /// </summary>
        /// <param name="Title">The title of the book to find.</param>
        /// <returns>An object full of all book's info.</returns>
        [HttpGet("Book/{Title}", Name = "FindBookByTitle")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsBookDTO> FindBookByTitle(string Title)
        {
            if(string.IsNullOrEmpty(Title))
                return BadRequest("Input is invalid");

            clsBook book = clsBook.Find(Title);

            if (book != null)
                return Ok(book.bookDTO);

            return NotFound($"Book with title {Title} is not found");
        }

        /// <summary>
        /// Adds a new book.
        /// </summary>
        /// <param name="addedBook">Book's info to be added.</param>
        /// <param name="AuthorFirstName">The first name of the book's author.</param>
        /// <param name="addedBook">The last name of the book's author.</param>
        /// <returns>An object full of all the added book's info if input is valid
        /// and no server error occurs.</returns>
        [HttpPost(Name = "AddNewBook")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult AddNewBook(clsBookDTO addedBook, string AuthorFirstName, string AuthorLastName)
        {
            if (!clsBook.IsValidInput(addedBook) ||
                string.IsNullOrEmpty(AuthorFirstName) || string.IsNullOrEmpty(AuthorLastName))
                return BadRequest("Input is invalid");

            clsBook newBook = new clsBook(new clsBookDTO(addedBook.BookID, addedBook.Title,
                addedBook.Genre, addedBook.ISBN, addedBook.Condition, addedBook.PublicationDate,
                addedBook.AvailabilityStatus, addedBook.Language));

            if(!newBook.Save(AuthorFirstName, AuthorLastName))
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new book." });

            addedBook.BookID = newBook.BookID;

            return CreatedAtRoute("FindBookByID", new { BookID = addedBook.BookID }, newBook.bookDTO);
        }

        /// <summary>
        /// Updates the condition (1: Good, 2: Damaged) of the specified book.
        /// </summary>
        /// <param name="BookID">The ID of the book to update its condition.</param>
        /// <param name="Condition">The new condition of the book.</param>
        /// <returns>Whether the book's condition is updated successfully or not.</returns>
        [HttpPatch("UpdateCondition/{BookID}/{Condition}", Name = "SetCondition")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> SetCondition(int BookID, byte Condition)
        {
            if (BookID < 0)
                return BadRequest("ID is invalid");

            if (Condition < 1 || Condition > 2)
                return BadRequest("Input is invalid, condition must be either 1 or 2.");

            clsBook book = clsBook.Find(BookID);

            if (book == null)
                return NotFound($"Book with id {BookID} is not found");

            return Ok(book.SetCondition((clsBook.enCondition)Condition));
        }

        /// <summary>
        /// Updates the availability status (1: Available, 2: Borrowed, 3: Reserved) of the specified book.
        /// </summary>
        /// <param name="BookID">The ID of the book to update its condition.</param>
        /// <param name="AvailablilityStatus">The new availability status of the book.</param>
        /// <returns>Whether the book's availability status is updated successfully or not.</returns>
        [HttpPatch("{BookID}/{AvailablilityStatus}", Name = "SetAvailabilityStatus")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> SetAvailabilityStatus(int BookID, byte AvailablilityStatus)
        {
            if (BookID < 0)
                return BadRequest("ID is invalid");

            if (AvailablilityStatus < 1 || AvailablilityStatus > 3)
                return BadRequest("Input is invalid, AvailablilityStatus must be either 1, 2, or 3.");

            clsBook book = clsBook.Find(BookID);

            if (book == null)
                return NotFound($"Book with id {BookID} is not found");

            return Ok(book.SetAvailabilityStatus((clsBook.enAvailabilityStatus)AvailablilityStatus));
        }

        /// <summary>
        /// Gets all books with all their info.
        /// </summary
        /// <returns>A list of books with their info.</returns>
        [HttpGet("All", Name = "GetAllBooksAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsBookDTO>>> GetAllBooksAsync()
        {
            List<clsBookGetAllDTO> books = await clsBook.GetAllBooksAsync();

            if (books.Count == 0 || books == null)
                return NotFound("Books are not found");

            return Ok(books);
        }

        /// <summary>
        /// Checks whether a specified ISBN exists in the system or not.
        /// </summary>
        /// <param name="ISBN">The ISBN to check if exists.</param>
        /// <returns>Whether the ISBN exists in the system or not.</returns>
        [HttpGet("DoesISBNExist/{ISBN}", Name = "DoesISBNExist")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> DoesISBNExist(string ISBN) =>
            string.IsNullOrEmpty(ISBN) ? BadRequest("Input is invalid") : Ok(clsBook.DoesISBNExist(ISBN));

        /// <summary>
        /// Deletes a book from the system.
        /// </summary>
        /// <param name="BookID">The ID of the book to be deleted.</param>
        /// <returns>Whether the book is deleted successfully or not.</returns>
        [HttpDelete("{BookID}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteBook(int BookID)
        {
            if (BookID < 0)
                return BadRequest("ID is invalid");

            if (clsBook.DeleteBook(BookID))
                return Ok("Book is deleted sucessfully");

            return NotFound($"Book with id {BookID} is not found");
        }

        /// <summary>
        /// Gets the ID of the author of a specified book.
        /// </summary>
        /// <param name="BookID">The ID of the book to get its author ID.</param>
        /// <returns>The ID of the Author of the specified book.</returns>
        [HttpGet("AuthorID/{BookID}", Name = "GetAuthorID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<int> GetAuthorID(int BookID)
        {
            if (BookID < 0)
                return BadRequest("ID is invalid");

            int AuthorID = clsBook.GetAuthorID(BookID);

            if (AuthorID == -1)
                return NotFound($"Book with id {BookID} is not found");

            return Ok(AuthorID);
        }
    }
}