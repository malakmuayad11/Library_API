using Library_Data;
using Models.DTOs;

namespace Library_Business
{
    public class clsUser
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public enum enRole { Admin = 1, Staff = 2 }
        public enRole Role { get; set; }
        public bool IsActive { get; set; }

        public clsUserDTO userDTO
        {
            get => new clsUserDTO(this.UserID, this.Username, this.Password, (byte)this.Role,
                this.IsActive, (int)this.Permissions);
        }

        public string RoleString
        {
            get => this.Role.ToString();
        }

        public int PermissionsInt
        {
            get => (int)this.Permissions;
        }

        public clsUserDTOImportantFields userDTOImportantFields
        {
            get => new clsUserDTOImportantFields(this.UserID, this.Username, (byte)this.Role, this.IsActive);
        }
        public enum enPermissions
        {
            eAll = -1, eManageMembers = 1, eManageBooks = 2, eManageCourses = 4,
            eManageUsers = 8, eManagePayments = 16
        }

        public enPermissions Permissions { get; set; }
        public enum enMode { AddNew = 1, Update = 2 }

        private enMode _Mode;

        public clsUser(clsUserDTO userDTO, enMode Mode = enMode.AddNew)
        {
            this.UserID = userDTO.UserID;
            this.Username = userDTO.Username;
            this.Password = userDTO.Password;
            this.Role = (enRole)userDTO.Role;
            this.Permissions = (enPermissions)userDTO.Permissions;
            this.IsActive = userDTO.IsActive;
            this._Mode = Mode;
        }

        public static clsUser Find(int UserID)
        {
            clsUserDTO userDTO = clsUserData.Find(UserID);

            if (userDTO != null)
                return new clsUser(userDTO, enMode.Update);

            return null;
        }

        public static clsUser Find(string Username, string Password)
        {
            clsUserDTO userDTO = clsUserData.Find(Username);

            if (userDTO != null && BCrypt.Net.BCrypt.Verify(Password, userDTO.Password))
                return new clsUser(userDTO, enMode.Update);

            return null;
        }

        public static clsUser Find(string Username)
        {
            clsUserDTO userDTO = clsUserData.Find(Username);

            if (userDTO != null)
                return new clsUser(userDTO, enMode.Update);

            return null;
        }

        public static bool UpdatePassword(int UserID, string Password) =>
            clsUserData.UpdatePassword(UserID, BCrypt.Net.BCrypt.HashPassword(Password));

        public static async Task<List<clsUserDTOImportantFields>> GetAllUsersAsync() => await clsUserData.GetAllUsersAsync();
    
        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(new clsUserDTO(this.UserID, this.Username,
                BCrypt.Net.BCrypt.HashPassword(this.Password), (byte)this.Role, this.IsActive, (int)this.Permissions));
            return UserID != -1;
        }
        public bool Save()
        {
            switch(this._Mode)
            {
                case enMode.AddNew:
                    {
                        if(_AddNewUser())
                        {
                            this._Mode = enMode.Update;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public static bool DoesUsernameExist(string Username) => clsUserData.DoesUsernameExist(Username);

        public static bool IsValidInput(clsUserDTO userDTO) =>
            !(userDTO == null || userDTO.UserID < 0 || string.IsNullOrEmpty(userDTO.Username) ||
                string.IsNullOrEmpty(userDTO.Password) || userDTO.Permissions < -1 ||
                userDTO.Role <= 0);

    }
}