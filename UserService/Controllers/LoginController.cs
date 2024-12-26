using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

        public LoginController(IConfiguration configuration, IUserAuthService userAuthService)
        {
            _configuration = configuration;
            _userAuthService = userAuthService;
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LoginModel userModel)
        {
            var user = _userAuthService.Authenticate(userModel);
            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(token);
            }
            return NotFound("User not found");
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
