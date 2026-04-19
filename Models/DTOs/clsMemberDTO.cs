namespace Models.DTOs
{
    public class clsMemberDTO
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public string? ImagePath { get; set; }
        public int MembershipTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsCancelled { get; set; }

        public clsMemberDTO(int memberID, string firstName, string secondName, string? thirdName,
            string lastName, DateTime dateOfBirth, string address, string phone, string? email,
            string? imagePath, int membershipTypeID, DateTime startDate, DateTime expiryDate, bool isCancelled)
        {
            MemberID = memberID;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
            Phone = phone;
            Email = email;
            ImagePath = imagePath;
            MembershipTypeID = membershipTypeID;
            StartDate = startDate;
            ExpiryDate = expiryDate;
            IsCancelled = isCancelled;
        }
    }
}
