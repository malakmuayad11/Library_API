namespace Models.DTOs
{
    public class clsPasswordRequestDTO
    {
        public string Password { get; set; }

        public clsPasswordRequestDTO(string Password) => this.Password = Password;
    }
}
