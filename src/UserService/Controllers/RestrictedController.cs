using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestrictedController : ControllerBase
    {
        [HttpGet]
        [Route("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            return currentUser == null ? Unauthorized() : Ok($"Hi you are an {currentUser.Role}");
        }
        [HttpGet]
        [Route("Users")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult UserEndPoint()
        {
            var currentUser = GetCurrentUser();
            return currentUser == null ? Unauthorized() : Ok($"Hi you are an {currentUser.Role}");
        }

        [HttpGet]
        [Route("Me")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Me()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return Unauthorized();
            return Ok(new { userId = currentUser.UserId, role = currentUser.Role.ToString() });
        }

        private UserModel? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return null;
            var userClaims = identity.Claims;
            var userIdClaim = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (userIdClaim == null || roleClaim == null) return null;
            return new UserModel
            {
                UserId = int.Parse(userIdClaim),
                Role = (UserRole)Enum.Parse(typeof(UserRole), roleClaim)
            };
        }
    }
}