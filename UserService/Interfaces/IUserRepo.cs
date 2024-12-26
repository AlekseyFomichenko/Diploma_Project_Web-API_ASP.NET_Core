using UserService.Db;

namespace UserService.Interfaces
{
    public interface IUserRepo
    {
        public void UserAdd(string name, string password, RoleId roleId);
        public RoleId UserCheck(string name, string password);
    }
}
