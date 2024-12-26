using UserService.Interfaces;

namespace UserService.Models
{
    public class AuthMock : IUserAuthService
    {
        public UserModel Authenticate(LoginModel model)
        {
            if (model.Name == "Admin" && model.Password == "passwordforadmin")
            {
                return new UserModel { UserName = model.Name, Password = model.Password, Role = UserRole.Admin };
            }
            if (model.Name == "User" && model.Password == "example")
            {
                return new UserModel { UserName = model.Name, Password = model.Password, Role = UserRole.User };
            }
            return null;
        }
    }
}
