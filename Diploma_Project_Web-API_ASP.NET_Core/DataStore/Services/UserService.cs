using Diploma_Project_Web_API_ASP.NET_Core.Abstractions;
using Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Diploma_Project_Web_API_ASP.NET_Core.DataStore.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _config;

        public UserService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public string CheckRole(string username, string password)
        {
            using (_appDbContext)
            {
                var entity = _appDbContext.Users
                    .FirstOrDefault(x => 
                    x.Login.ToLower()
                    .Equals(username.ToLower()) && 
                    x.Password
                    .Equals(password));

                if (entity == null) return string.Empty;
                else
                {
                    var user = new UserModel
                    {
                        UserName = entity.Login,
                        Password = entity.Password,
                        Role = entity.RoleType
                    };
                    return GenerateToken(user);
                }
            }
        }

        public Guid UserAdd(string username, string password, UserRole roleId)
        {
            var users = new List<UserEntity>();
            using (_appDbContext)
            {
                var userExit = _appDbContext.Users.Where(x => x.Login.ToLower().Equals(username.ToLower()));
                UserEntity userEntity = null;
                if (userExit == null)
                {
                    userEntity = new UserEntity
                    {
                        Id = Guid.NewGuid(),
                        Login = username,
                        Password = password,
                        RoleType = roleId
                    };
                    return userEntity.Id;
                }
                else return default;
            }
        }

        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
