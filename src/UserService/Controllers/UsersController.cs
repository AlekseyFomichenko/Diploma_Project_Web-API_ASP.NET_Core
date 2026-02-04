using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Db;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IJwtTokenService _jwtTokenService;

        public UsersController(IUserRepo userRepo, IJwtTokenService jwtTokenService)
        {
            _userRepo = userRepo;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("google-ensure")]
        public IActionResult GoogleEnsure([FromBody] GoogleEnsureRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest("Email required");
            var (userId, roleId) = _userRepo.EnsureUserByEmail(request.Email, request.Name);
            var role = roleId == RoleId.Admin ? UserRole.Admin : UserRole.User;
            var token = _jwtTokenService.GenerateToken(userId, role);
            return Ok(new GoogleEnsureResponse { Token = token });
        }
    }

    public record GoogleEnsureRequest
    {
        public string? Email { get; init; }
        public string? Name { get; init; }
    }

    public record GoogleEnsureResponse
    {
        public string Token { get; init; } = null!;
    }
}
