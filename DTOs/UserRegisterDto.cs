namespace Hospital_Management.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

        public int? UId { get; set; }

        public int? DId { get; set; }
    }
}
