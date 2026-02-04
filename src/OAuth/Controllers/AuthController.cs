using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace OAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public AuthController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpPost("exchange")]
        public IActionResult Exchange([FromBody] ExchangeRequest request)
        {
            if (string.IsNullOrEmpty(request?.Code))
                return BadRequest(new { error = "code required" });
            if (!_cache.TryGetValue(request.Code, out string? token) || string.IsNullOrEmpty(token))
                return BadRequest(new { error = "invalid or expired code" });
            _cache.Remove(request.Code);
            return Ok(new ExchangeResponse { Token = token });
        }
    }

    public record ExchangeRequest
    {
        public string? Code { get; init; }
    }

    public record ExchangeResponse
    {
        public string Token { get; init; } = null!;
    }
}
