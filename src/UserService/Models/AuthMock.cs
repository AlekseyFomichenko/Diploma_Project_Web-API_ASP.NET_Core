using UserService.Interfaces;

namespace UserService.Models
{
    public sealed class AuthMock : IUserAuthService
    {
        public UserModel? Authenticate(LoginModel model)
        {
            if (model.Email == "admin@test" && model.Password == "passwordforadmin")
                return new UserModel { UserId = 1, Role = UserRole.Admin };
            if (model.Email == "user@test" && model.Password == "example")
                return new UserModel { UserId = 2, Role = UserRole.User };
            return null;
        }
    }
}
