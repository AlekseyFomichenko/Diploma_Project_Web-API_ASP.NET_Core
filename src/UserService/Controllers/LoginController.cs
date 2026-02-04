using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Db;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IJwtTokenService _jwtTokenService;

        private static UserRole RoleToUserRole(RoleId id) =>
            id == RoleId.Admin ? UserRole.Admin : UserRole.User;

        public LoginController(IUserRepo userRepo, IJwtTokenService jwtTokenService)
        {
            _userRepo = userRepo;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] LoginModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.Email) || string.IsNullOrEmpty(userModel.Password))
                return BadRequest("Email and Password required");
            try
            {
                _userRepo.UserAdd(userModel.Email, userModel.Password, RoleId.User);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult LogIn([FromBody] LoginModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.Email) || string.IsNullOrEmpty(userModel.Password))
                return BadRequest("Email and Password required");
            try
            {
                var (roleId, userId) = _userRepo.UserCheck(userModel.Email, userModel.Password);
                var role = RoleToUserRole(roleId);
                var token = _jwtTokenService.GenerateToken(userId, role);
                return Ok(new { token });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
