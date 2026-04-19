namespace Models.DTOs
{
    public class clsUserDTOImportantFields
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public byte Role {  get; set; }
        public bool IsActive { get; set; }

        public clsUserDTOImportantFields(int UserID, string Username, byte Role, bool IsActive)
        {
            this.UserID = UserID;
            this.Username = Username;
            this.Role = Role;
            this.IsActive = IsActive;
        }
    }
}
