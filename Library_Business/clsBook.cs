using Library_Data;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_Business
{
    public class clsBook
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public enum enCondition { Good = 1, Damaged = 2 }
        public enCondition Condition { get; set; }
        public DateTime PublicationDate { get; set; }

        public enum enAvailabilityStatus { Available = 1, Borrowed = 2, Reserved = 3 }
        public enAvailabilityStatus AvailabilityStatus { get; set; }

        public enum enLanguage { Arabic = 1, English = 2 }
        public enLanguage Language { get; set; }

        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode;

        public clsBookDTO bookDTO
        {
            get => new clsBookDTO(this.BookID, this.Title, this.Genre, this.ISBN,
                (byte)this.Condition, this.PublicationDate, (byte)this.AvailabilityStatus, (byte)this.Language);
        }

        public static string GetConditionString(byte condition)
        {
            switch (condition) { 
                case 1: return "Good";
                case 2: return "Damaged";
                default: return "Unknown";
            }
        }

        public static string GetAvaiabilityStatus(byte AvailabilityStatus)
        {
            switch (AvailabilityStatus)
            {
                case 1: return "Available";
                case 2: return "Borrowed";
                case 3: return "Reserved";
                default: return "Unknown";
            }
        }

        public clsBook(clsBookDTO bookDTO, enMode Mode = enMode.AddNew)
        {
            this.BookID = bookDTO.BookID;
            this.Title = bookDTO.Title;
            this.Genre = bookDTO.Genre;
            this.ISBN = bookDTO.ISBN;
            this.Condition = (enCondition)bookDTO.Condition;
            this.PublicationDate = bookDTO.PublicationDate;
            this.AvailabilityStatus = (enAvailabilityStatus)bookDTO.AvailabilityStatus;
            this.Language = (enLanguage)bookDTO.Language;
            this._Mode = Mode;
        }

        private bool _AddAuthorToBook(string AuthorFirstName, string AuthorLastName)
        {
            if (!clsAuthor.IsAuthorExists(AuthorFirstName, AuthorLastName))
            {
                clsAuthor author = new clsAuthor(new clsAuthorDTO(-1, AuthorFirstName, AuthorLastName));

                if (!author.Save())
                    return false;
            }
            return clsBookData.AddAuthorToBook(clsAuthor.Find(AuthorFirstName, AuthorLastName)
                .AuthorID, this.BookID);
        }

        private bool _AddNewBook(string AuthorFirstName, string AuthorLastName)
        {
            this.BookID = clsBookData.AddNewBook(new clsBookDTO(this.BookID, this.Title, this.Genre,
                this.ISBN, (byte)this.Condition, this.PublicationDate, (byte)this.AvailabilityStatus, (byte)this.Language));
            if (this.BookID == -1) return false;
            if (!_AddAuthorToBook(AuthorFirstName, AuthorLastName)) return false;
            return true;
        }

        public bool Save(string AuthorFirstName, string AuthorLastName)
        {
            switch (this._Mode)
            {
                case enMode.AddNew:

                    if (_AddNewBook(AuthorFirstName, AuthorLastName))
                    {
                        this._Mode = enMode.Update;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool SetCondition(enCondition Condition) =>
            clsBookData.UpdateCondition(this.BookID, (byte)Condition);

        public bool SetAvailabilityStatus(enAvailabilityStatus Status) =>
            clsBookData.UpdateAvailabilityStatus(BookID, (byte)Status);

        public async static Task<List<clsBookGetAllDTO>> GetAllBooksAsync() =>
            await clsBookData.GetAllBooksAsync();

        public static bool DoesISBNExist(string ISBN) => clsBookData.DoesISBNExist(ISBN);

        public static clsBook Find(int BookID)
        {
            clsBookDTO bookDTO = clsBookData.Find(BookID);

            if (bookDTO != null)
                return new clsBook(bookDTO, enMode.Update);

            return null;
        }

        public static clsBook Find(string Title)
        {
            clsBookDTO bookDTO = clsBookData.Find(Title);

            if (bookDTO != null)
                return new clsBook(bookDTO, enMode.Update);

            return null;
        }

        public static bool DeleteBook(int BookID) => clsBookData.DeleteBook(BookID);

        public static int GetAuthorID(int BookID) => clsBookData.GetAuthorID(BookID);

        public static bool IsValidInput(clsBookDTO bookDTO) =>
            !(bookDTO.BookID < 0 || string.IsNullOrEmpty(bookDTO.Title) ||
            string.IsNullOrEmpty(bookDTO.Genre) || string.IsNullOrEmpty(bookDTO.ISBN)
            || bookDTO.Condition < 0 || bookDTO.PublicationDate == null || bookDTO.AvailabilityStatus < 0
            || bookDTO.Language < 0);
    }
}