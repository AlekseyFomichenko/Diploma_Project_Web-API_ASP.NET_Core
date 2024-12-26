using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Db;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers
{
    public static class RsaTools
    {
        public static RSA GetPrivateKey()
        {
            var f = File.ReadAllText("rsa.private_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(f);
            return rsa;
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserAuthService _userAuthService;
        private readonly IUserRepo _userRepo;

        private static UserRole RoleToUserRole(RoleId id)
        {
            return id == RoleId.Admin ? UserRole.Admin : UserRole.User;
        }
        public LoginController(IConfiguration configuration, IUserAuthService userAuthService, IUserRepo userRepo)
        {
            _configuration = configuration;
            _userAuthService = userAuthService;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddUser")]
        public IActionResult AddUser([FromBody] LoginModel userModel)
        {
            try
            {
                _userRepo.UserAdd(userModel.Name, userModel.Password, Db.RoleId.User);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LoginModel userModel)
        {
            try
            {
                var roleId = _userRepo.UserCheck(userModel.Name, userModel.Password);
                var user = new UserModel { UserName = userModel.Name, Role = RoleToUserRole(roleId)};
                var token = GenerateToken(user);
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        private string GenerateToken(UserModel user)
        {
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var key = new RsaSecurityKey(RsaTools.GetPrivateKey());
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
