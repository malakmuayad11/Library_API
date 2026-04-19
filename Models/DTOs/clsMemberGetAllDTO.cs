namespace Models.DTOs
{
    public class clsMemberGetAllDTO
    {
        public int MemberID { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public string MembershipName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }

        public clsMemberGetAllDTO(int memberID, string fullName, DateTime dateOfBirth, string phone, string? email,
           string membershipName, DateTime startDate, DateTime expiryDate, string status)
        {
            MemberID = memberID;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Phone = phone;
            Email = email;
            MembershipName = membershipName;
            StartDate = startDate;
            ExpiryDate = expiryDate;
            Status = status;
        }
    }
}
