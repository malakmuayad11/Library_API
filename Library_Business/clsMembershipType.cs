using Library_Data;
using Models.DTOs;
using System.Threading.Tasks;

namespace Library_Business
{
    public class clsMembershipType
    {
        public int MemberhipTypeID { get; set; }
        public string MembershipName { get; set; }
        public byte NumberOfAllowedBooksToBorrow { get; set; }

        public clsMembershipTypeDTO membershipTypeDTO
        {
            get => new clsMembershipTypeDTO(this.MemberhipTypeID, this.MembershipName, this.NumberOfAllowedBooksToBorrow);
        }

        public clsMembershipType(clsMembershipTypeDTO membershipTypeDTO)
        {
            this.MemberhipTypeID = membershipTypeDTO.MemberhipTypeID;
            this.MembershipName = membershipTypeDTO.MembershipName;
            this.NumberOfAllowedBooksToBorrow = membershipTypeDTO.NumberOfAllowedBooksToBorrow;
        }

        public static async Task<List<clsMembershipTypeDTO>> GetAllMembershipTypesAsync() =>
            await clsMembershipTypeData.GetAllMembershipTypesAsync();

        public static clsMembershipType Find(int MembershipTypeID)
        {
            clsMembershipTypeDTO membershipTypeDTO = clsMembershipTypeData.Find(MembershipTypeID);

            if (membershipTypeDTO != null)
                return new clsMembershipType(membershipTypeDTO);

            return null;
        }
    }
}