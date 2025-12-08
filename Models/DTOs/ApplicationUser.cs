namespace Models.DTOs
{
    public class Register
    {
        
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
    public class Login
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string Email { get; set; } = null!;
    }   
}
