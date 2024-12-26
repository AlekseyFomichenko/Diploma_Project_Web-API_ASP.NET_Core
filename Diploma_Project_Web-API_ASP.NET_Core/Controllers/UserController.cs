using Diploma_Project_Web_API_ASP.NET_Core.Abstractions;
using Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Diploma_Project_Web_API_ASP.NET_Core.Controllers
{
    [ApiController]
    [Route("controller")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string login, string password)
        {
            var token = _userService.CheckRole(login, password);
            return token.IsNullOrEmpty() ? NotFound("User not found") : Ok(token);
        }

        
    }
}
