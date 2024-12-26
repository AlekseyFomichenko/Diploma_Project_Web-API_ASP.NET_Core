using UserService.Models;

namespace UserService.Interfaces
{
    public interface IUserAuthService
    {
        UserModel Authenticate(LoginModel model);  
    }
}
