using Library_Data;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_Business
{
    public class clsLoan
    {
        public int LoanID { get; set; }
        public int BookID { get; set; }
        public clsBook BookInfo;
        public int MemberID { get; set; }
        public clsMember MemberInfo;

        public DateTime LoanStartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public float? FineAmount { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUser CreatedByUserInfo;

        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode;

        public clsLoanDTO loanDTO
        {
            get => new clsLoanDTO(this.LoanID, this.BookID, this.MemberID,
                this.LoanStartDate, this.DueDate, this.ReturnDate, this.FineAmount, this.CreatedByUserID);
        }

        public clsLoan(clsLoanDTO loanDTO, enMode Mode = enMode.AddNew)
        {
            this.LoanID = loanDTO.LoanID;
            this.BookID = loanDTO.BookID;
            this.MemberID = loanDTO.MemberID;
            this.LoanStartDate = loanDTO.LoanStartDate;
            this.DueDate = loanDTO.DueDate;
            this.ReturnDate = loanDTO.ReturnDate;
            this.FineAmount = loanDTO.FineAmount;
            this.CreatedByUserID = loanDTO.CreatedByUserID;
            this._Mode = Mode;
        }

        public async static Task<List<clsLoanGetAllDTO>> GetAllLoansAsync() => await clsLoanData.GetAllLoansAsync();

        private bool _AddNewLoan()
        {
            this.LoanID = clsLoanData.AddNewLoan(this.BookID, this.MemberID, this.CreatedByUserID);
            return this.LoanID != -1;
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLoan())
                    {
                        this._Mode = enMode.Update;
                        return true;
                    }
                    break;
            }
            return false;
        }

        private float? _CalculateLoanAmount(DateTime DueDate, DateTime ReturnDate)
        {
            TimeSpan t = new TimeSpan();
            if (ReturnDate < DueDate)
                return null;
            t = ReturnDate - DueDate;
            return t.Days * 5; // It is specified by the system that for each late day, pay 5.
        }

        private bool _AddFine()
        {
            if (FineAmount.HasValue)
            {
                clsFine Fine = new clsFine(new clsFineDTO(-1, this.MemberID, this.LoanID, Convert.ToDecimal(this.FineAmount), false));
                Fine.MemberID = this.MemberID;
                Fine.LoanID = this.LoanID;
                Fine.FineAmount = Convert.ToDecimal(FineAmount.Value);
                return Fine.Save();
            }
            return true;
        }

        public bool Return(int LoanID)
        {
            clsBook ReturnedBook = clsBook.Find(this.BookID);
            if (ReturnedBook == null)
                return false;
            ReturnedBook.SetAvailabilityStatus(clsBook.enAvailabilityStatus.Available);
            float? FineAmount = _CalculateLoanAmount(this.DueDate, DateTime.Now);
            if (!_AddFine()) return false;
            return clsLoanData.ReturnLoan(LoanID, DateTime.Now, FineAmount);
        }

        public static clsLoan Find(int LoanID)
        {
            clsLoanDTO loanDTO = clsLoanData.Find(LoanID);

            if (loanDTO != null)
                return new clsLoan(loanDTO, enMode.Update);

            return null;
        }

        public static clsLoan FindByMemberID(int MemberID)
        {
            clsLoanDTO loanDTO = clsLoanData.FindByMemberID(MemberID);

            if (loanDTO != null)
                return new clsLoan(loanDTO, enMode.Update);

            return null;
        }

        public static bool CanReturnBook(int LoanID) => clsLoanData.CanReturnBook(LoanID);

        public static bool CanExtendLoan(int LoanID)
        {
            clsLoan loan = Find(LoanID);
            if (loan == null)
                return false;

            return loan.ReturnDate.HasValue;
        }

        public static bool ExtendDueDate(int LoanID, DateTime DueDate) =>
            clsLoanData.UpdateDueDate(LoanID, DueDate.AddDays(14));

        public static bool IsValidLoanInput(clsLoanDTO loanDTO) =>
            !(loanDTO.LoanID < 0 || loanDTO.BookID < 0 ||
            loanDTO.MemberID < 0 || loanDTO.CreatedByUserID < 0);
    }
}