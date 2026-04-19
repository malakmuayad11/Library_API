namespace Models.DTOs
{
    public class clsLoanDTO
    {
        public int LoanID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }
        public DateTime LoanStartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public float? FineAmount { get; set; }
        public int CreatedByUserID { get; set; }

        public clsLoanDTO(int loanID, int bookID, int memberID, DateTime loanStartDate,
            DateTime dueDate, DateTime? returnDate, float? fineAmount, int createdByUserID)
        {
            LoanID = loanID;
            BookID = bookID;
            MemberID = memberID;
            LoanStartDate = loanStartDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            FineAmount = fineAmount;
            CreatedByUserID = createdByUserID;
        }
    }
}
