using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public record LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; init; }

        public string? Name { get; init; }
    }
}
