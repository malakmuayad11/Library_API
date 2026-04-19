namespace Models.DTOs
{
    public class clsUserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte Role { get; set; }
        public bool IsActive { get; set; }
        public int Permissions { get; set; }

        public clsUserDTO(int userID, string username, string password,
            byte role, bool isActive, int permissions)
        {
            UserID = userID;
            Username = username;
            Password = password;
            Role = role;
            IsActive = isActive;
            Permissions = permissions;
        }
    }
}
