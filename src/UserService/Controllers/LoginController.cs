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

        private static bool ValidatePassword(string? password, out string? errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "Password is required.";
                return false;
            }
            if (password.Length < 6 || password.Length > 64)
            {
                errorMessage = "Password must be between 6 and 64 characters.";
                return false;
            }
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));
            if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                errorMessage = "Password must contain uppercase, lowercase, digit and special character.";
                return false;
            }
            return true;
        }

        public LoginController(IUserRepo userRepo, IJwtTokenService jwtTokenService)
        {
            _userRepo = userRepo;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] LoginModel userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!ValidatePassword(userModel.Password, out var passwordError))
                return BadRequest(passwordError);
            try
            {
                _userRepo.UserAdd(userModel.Email!, userModel.Password!, RoleId.User);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("already exists"))
                    return Conflict(e.Message);
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("AddAdmin")]
        public IActionResult AddAdmin([FromBody] LoginModel userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!ValidatePassword(userModel.Password, out var passwordError))
                return BadRequest(passwordError);
            try
            {
                _userRepo.AddAdmin(userModel.Email!, userModel.Password!);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("already exists"))
                    return Conflict(e.Message);
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
