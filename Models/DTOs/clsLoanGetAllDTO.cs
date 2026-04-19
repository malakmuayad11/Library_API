using System;

namespace Models.DTOs
{
    public class clsLoanGetAllDTO
    {
        public int LoanID { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public DateTime LoanStartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string ReturnDate { get; set; }
        public string FineAmount { get; set; }
        public string Username { get; set; }

        public clsLoanGetAllDTO(int loanID, string title, string fullName, DateTime loanStartDate,
            DateTime dueDate, string returnDate, string fineAmount, string username)
        {
            LoanID = loanID;
            Title = title;
            FullName = fullName;
            LoanStartDate = loanStartDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            FineAmount = fineAmount;
            Username = username;
        }
    }
}
