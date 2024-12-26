namespace UserService.Models
{
    public class LoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
