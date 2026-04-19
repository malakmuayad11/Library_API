namespace Models.DTOs
{
    public class clsBookDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public byte Condition { get; set; }
        public DateTime PublicationDate { get; set; }
        public byte AvailabilityStatus { get; set; }
        public byte Language { get; set; }

        public clsBookDTO(int bookID, string title, string genre, string iSBN, byte condition,
            DateTime publicationDate, byte availabilityStatus, byte language)
        {
            BookID = bookID;
            Title = title;
            Genre = genre;
            ISBN = iSBN;
            Condition = condition;
            PublicationDate = publicationDate;
            AvailabilityStatus = availabilityStatus;
            Language = language;
        }
    }
}
