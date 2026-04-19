using Library_Data;
using Models.DTOs;

namespace Library_Business
{
    public class clsAuthor
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode;

        public clsAuthorDTO authorDTO
        {
            get => new clsAuthorDTO(this.AuthorID, this.FirstName, this.LastName);
        }

        public clsAuthor(clsAuthorDTO authorDTO, enMode Mode = enMode.AddNew)
        {
            this.AuthorID = authorDTO.AuthorID;
            this.FirstName = authorDTO.FirstName;
            this.LastName = authorDTO.LastName;
            this._Mode = enMode.AddNew;
        }

        private bool _AddNewAuthor()
        {
            this.AuthorID = clsAuthorData.AddNewAuthor(new clsAuthorDTO(this.AuthorID, this.FirstName, this.LastName));
            return this.AuthorID != -1;
        }

        public bool Save()
        {
            switch (this._Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAuthor())
                    {
                        this._Mode = enMode.Update;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public static bool IsAuthorExists(string FirstName, string LastName) =>
            clsAuthorData.IsAuthorExists(FirstName, LastName);

        public static clsAuthor FindByBookID(int BookID)
        {
            clsAuthorDTO authorDTO = clsAuthorData.FindAuthorByBookID(BookID);

            if (authorDTO != null)
                return new clsAuthor(authorDTO, enMode.Update);

            return null;
        }

        public static clsAuthor Find(string FirstName, string LastName)
        {
            clsAuthorDTO authorDTO = clsAuthorData.Find(FirstName, LastName);

            if (authorDTO != null)
                return new clsAuthor(authorDTO, enMode.Update);

            return null;
        }
    }
}