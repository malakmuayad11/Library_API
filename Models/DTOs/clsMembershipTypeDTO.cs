namespace Models.DTOs
{
    public class clsMembershipTypeDTO
    {
        public int MemberhipTypeID { get; set; }
        public string MembershipName { get; set; }
        public byte NumberOfAllowedBooksToBorrow { get; set; }

        public clsMembershipTypeDTO(int memberhipTypeID, string membershipName, 
            byte numberOfAllowedBooksToBorrow)
        {
            MemberhipTypeID = memberhipTypeID;
            MembershipName = membershipName;
            NumberOfAllowedBooksToBorrow = numberOfAllowedBooksToBorrow;
        }
    }
}
