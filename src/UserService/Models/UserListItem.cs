namespace UserService.Models
{
    public record UserListItem
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public string? Name { get; init; }
        public string Role { get; init; } = null!;
    }
}
