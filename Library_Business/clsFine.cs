using System.Collections.Generic;
using System.Threading.Tasks;
using Library_Data;
using Models.DTOs;

namespace Library_Business
{
    public class clsFine
    {
        public int FineID { get; set; }
        public int MemberID { get; set; }
        public clsMember MemberInfo;
        public int LoanID { get; set; }
        public clsLoan LoanInfo;
        public decimal FineAmount { get; set; }
        public bool IsPaid { get; set; }

        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode { get; set; }

        public clsFineDTO fineDTO
        {
            get => new clsFineDTO(this.FineID, this.MemberID,
                this.LoanID, this.FineAmount, this.IsPaid);
        }

        public clsFine(clsFineDTO fineDTO, enMode Mode = enMode.AddNew)
        {
            this.FineID = fineDTO.FineID;
            this.MemberID = fineDTO.MemberID;
            this.LoanID = fineDTO.LoanID;
            this.FineAmount = fineDTO.FineAmount;
            this.IsPaid = fineDTO.IsPaid;
            this._Mode = Mode;
        }

        private bool _AddNewFine()
        {
            this.FineID = clsFineData.AddNewFine(new clsFineDTO(this.FineID, this.MemberID, this.LoanID,
                this.FineAmount, this.IsPaid));
            return (this.FineID != -1);
        }

        public bool Save()
        {
            switch (this._Mode)
            {
                case enMode.AddNew:
                    if (_AddNewFine())
                    {
                        this._Mode = enMode.Update;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public async static Task<List<clsFineDTO>> GetAllFinesAsync()
            => await clsFineData.GetAllFinesAsync();

        public static bool UpdatePaymentStatus(int FineID, bool IsPaid) =>
            clsFineData.UpdatePaymentStatus(FineID, IsPaid);

        public static bool UpdateFineAmount(int FineID, float Amount) =>
            clsFineData.UpdateFineAmount(FineID, Amount);

        public static bool PayFines(int FineID) => UpdatePaymentStatus(FineID, true);

        public static decimal? GetMemberUnpaidFees(int MemberID) =>
            clsFineData.GetMemberUnpaidFines(MemberID);

        public static bool IsValidInput(clsFineDTO fineDTO) =>
            !(fineDTO.FineID < 0 || fineDTO.MemberID < 0 ||
            fineDTO.LoanID < 0 || fineDTO.FineAmount < 0);
    }
}