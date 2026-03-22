using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace OAuth.Controllers
{
    public class GoogleLoginController : Controller
    {
        private readonly ILogger<GoogleLoginController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public GoogleLoginController(
            ILogger<GoogleLoginController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cache = cache;
        }

        public IActionResult Index()
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse", "GoogleLogin")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
                return BadRequest();

            var principal = authenticateResult.Principal;
            if (principal?.Identities.FirstOrDefault()?.AuthenticationType?.ToLowerInvariant() != "google")
                return RedirectToAction("Index", "Home");

            var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? principal.FindFirst("email")?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? principal.FindFirst("name")?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email not provided by Google");

            var client = _httpClientFactory.CreateClient("UserService");
            var response = await client.PostAsJsonAsync("/api/users/google-ensure", new { email, name });
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("UserService google-ensure failed: {Status}", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Failed to get token");
            }
            var body = await response.Content.ReadFromJsonAsync<GoogleEnsureResponse>();
            var token = body?.Token;
            if (string.IsNullOrEmpty(token))
                return StatusCode(500, "No token in response");

            var code = Guid.NewGuid().ToString("N");
            _cache.Set(code, token, TimeSpan.FromMinutes(2));
            var redirectUri = (_configuration["ClientRedirectUri"] ?? "/").TrimEnd('/');
            return Redirect(redirectUri + "?code=" + code);
        }

        public async Task<IActionResult> SignOutFromGoogleLogin()
        {
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                    string? value = Request.Cookies[cookie.Key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        // Логируем или выводим предупреждение, что кука не была удалена
                        _logger.LogWarning($"Кука {cookie.Key} не была удалена.");
                    }
                    await Task.Delay(200); // Ждём небольшую паузу, чтобы операция точно завершилась
                }
            }
            await HttpContext.SignOutAsync("External");
            return RedirectToAction("Index", "Home");
        }
    }

    internal class GoogleEnsureResponse
    {
        public string Token { get; set; } = null!;
    }
}