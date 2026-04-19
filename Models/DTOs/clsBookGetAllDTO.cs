namespace Models.DTOs
{
    public class clsBookGetAllDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Condition { get; set; }
        public string AvailabilityStatus { get; set; }
        public string FullName { get; set; }

        public clsBookGetAllDTO(int bookID, string title, string genre, string condition,
            string availabilityStatus, string fullName)
        {
            BookID = bookID;
            Title = title;
            Genre = genre;
            Condition = condition;
            AvailabilityStatus = availabilityStatus;
            FullName = fullName;
        }
    }
}
