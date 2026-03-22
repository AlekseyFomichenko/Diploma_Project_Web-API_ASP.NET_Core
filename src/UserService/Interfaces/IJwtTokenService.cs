using UserService.Models;

namespace UserService.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(int userId, UserRole role);
    }
}
