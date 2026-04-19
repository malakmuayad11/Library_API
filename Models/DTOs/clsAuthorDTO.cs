namespace Models.DTOs
{
    public class clsAuthorDTO
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public clsAuthorDTO(int authorID, string firstName, string lastName)
        {
            AuthorID = authorID;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
