namespace Models.DTOs
{
    public class clsFineDTO
    {
        public int FineID { get; set; }
        public int MemberID { get; set; }
        public int LoanID { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsPaid { get; set; }

        public clsFineDTO(int fineID, int memberID, int loanID, decimal fineAmount, bool isPaid)
        {
            FineID = fineID;
            MemberID = memberID;
            LoanID = loanID;
            FineAmount = fineAmount;
            IsPaid = isPaid;
        }
    }
}
