using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OAuth.Controllers
{
    public class GoogleLoginController : Controller
    {
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
            if (authenticateResult.Succeeded)
                return BadRequest();

            if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower() == "google")
                    {
                if (authenticateResult.Principal != null)
                {
                    var claimsIdentity = new ClaimsIdentity("Application");
                    if (authenticateResult.Principal != null)
                    {
                        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Name));
                        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email));
                        await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SignOutFromGoogleLogin()
        {
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                foreach (var cookie in siteCookies)
                    Response.Cookies.Delete(cookie.Key);
            }
            await HttpContext.SignOutAsync("External");
            return RedirectToAction("Index", "Home");
        }
    }
}