using Library_Data;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_Business
{
    public class clsMember
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
        public clsMembershipType MembershipTypeInfo;
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsCancelled { get; set; }

        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode;

        public string FullName
        {
            get
                => this.FirstName + " " + this.SecondName + " "
                    + this.ThirdName ?? string.Empty + " " + this.LastName;
        }

        public clsMemberDTO memberDTO
        {
            get => new clsMemberDTO(this.MemberID, this.FirstName, 
                this.SecondName, this.ThirdName, this.LastName, this.DateOfBirth,
                this.Address, this.Phone, this.Email, this.ImagePath, this.MembershipTypeID,
                this.StartDate, this.ExpiryDate, this.IsCancelled);
        }

        public clsMember(clsMemberDTO memberDTO, enMode Mode = enMode.AddNew)
        {
            this.MemberID = memberDTO.MemberID;
            this.FirstName = memberDTO.FirstName;
            this.SecondName = memberDTO.SecondName;
            this.ThirdName = memberDTO.ThirdName;
            this.LastName = memberDTO.LastName;
            this.DateOfBirth = memberDTO.DateOfBirth;
            this.Address = memberDTO.Address;
            this.Phone = memberDTO.Phone;
            this.Email = memberDTO.Email;
            this.ImagePath = memberDTO.ImagePath;
            this.MembershipTypeID = memberDTO.MembershipTypeID;
            this.StartDate = memberDTO.StartDate;
            this.ExpiryDate = memberDTO.ExpiryDate;
            this.IsCancelled = memberDTO.IsCancelled;
            this._Mode = Mode;
        }

        private bool _AddNewMember()
        {
            this.MemberID = clsMemberData.AddNewMember(memberDTO);
            return this.MemberID != -1;
        }

        private bool _UpdateMember() => clsMemberData.UpdateMember(memberDTO);

        public bool Save()
        {
            switch (this._Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewMember())
                        {
                            this._Mode = enMode.Update;
                            return true;
                        }
                    }
                    break;
                case enMode.Update:
                    return this._UpdateMember();
            }
            return false;
        }

        public static clsMember Find(int MemberID)
        {
            clsMemberDTO memberDTO = clsMemberData.Find(MemberID);

            if(memberDTO != null)
                return new clsMember(memberDTO, enMode.Update);

            return null;
        }

        public static bool UpdateCancel(int MemberID, bool IsCancelled) => 
            clsMemberData.UpdateCancel(MemberID, IsCancelled);

        public static async Task<List<clsMemberGetAllDTO>> GetAllMembersAsync() => await clsMemberData.GetAllMembersAsync();

        public static bool RenewMembership(int MemberID) => clsMemberData.RenewMembership(MemberID);

        public static int GetNumberOfBorrowedBook(int MemberID) => clsMemberData.GetNumberOfBorrowedBook(MemberID);

        public static bool IsValidMemberInput(clsMemberDTO memberDTO) =>
            !(memberDTO.MemberID < 0 || string.IsNullOrEmpty(memberDTO.FirstName) ||
                string.IsNullOrEmpty(memberDTO.SecondName) || string.IsNullOrEmpty(memberDTO.LastName)
                || string.IsNullOrEmpty(memberDTO.Address) || string.IsNullOrEmpty(memberDTO.Phone)
            || memberDTO.MembershipTypeID < 0 || memberDTO.MembershipTypeID > 3);
    }
}