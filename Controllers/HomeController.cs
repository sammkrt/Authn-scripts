using Authn.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Authn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("denied")]
        public IActionResult Denied() 
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Secured()
        {

            var idToken = await HttpContext.GetTokenAsync("id_token");
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpGet("login/{provider}")]
        public IActionResult LoginExternal([FromRoute]string provider,[FromQuery]string returnUrl)
        {
            if (User != null && User.Identities.Any(identity => identity.IsAuthenticated)) 
            {
                RedirectToAction("", "Home");
            }

            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            var authenticationProperties = new AuthenticationProperties { RedirectUri = returnUrl };
            return new ChallengeResult(provider, authenticationProperties);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (username == "samet" && password == "123")
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("username", username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier,username));
                claims.Add(new Claim(ClaimTypes.Name,"Samet Kurt"));
                

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var items = new Dictionary<string, string>();
                items.Add(".AuthScheme", CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties(items);
                await HttpContext.SignInAsync(claimsPrincipal,properties);
                return Redirect(returnUrl);
            }
            TempData["Error"] = "Error. Username or password is invalid";
            return View("login");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}