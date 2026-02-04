namespace UserService.Models
{
    public record UserModel
    {
        public int UserId { get; init; }
        public UserRole Role { get; init; }
    }
}
