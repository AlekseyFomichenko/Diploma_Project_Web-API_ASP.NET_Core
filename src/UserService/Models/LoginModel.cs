namespace UserService.Models
{
    public record LoginModel
    {
        public string? Email { get; init; }
        public string? Password { get; init; }
        public string? Name { get; init; }
    }
}
