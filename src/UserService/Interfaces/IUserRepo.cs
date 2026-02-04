using UserService.Db;

namespace UserService.Interfaces
{
    public interface IUserRepo
    {
        void UserAdd(string email, string password, RoleId roleId);
        (RoleId RoleId, int UserId) UserCheck(string email, string password);
        (int UserId, RoleId RoleId) EnsureUserByEmail(string email, string? name);
    }
}
