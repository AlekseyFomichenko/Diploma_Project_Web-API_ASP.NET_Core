using UserService.Db;
using UserService.Models;

namespace UserService.Interfaces
{
    public interface IUserRepo
    {
        void UserAdd(string email, string password, RoleId roleId);
        void AddAdmin(string email, string password);
        (RoleId RoleId, int UserId) UserCheck(string email, string password);
        (int UserId, RoleId RoleId) EnsureUserByEmail(string email, string? name);
        IEnumerable<UserListItem> GetAllUsers();
        void DeleteUser(int userId);
    }
}
