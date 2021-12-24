using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using HostAccessibilityCheckingSite.Data.Models;

namespace HostAccessibilityCheckingSite.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [NonAction]
        private ClaimsPrincipal generateClaimsPrincipal(User user)
        {
            var claims = new List<Claim>()
                        {
                            new Claim("username", user.Username),
                            new Claim("userId", user.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Name, user.Username),
                        };

            if (user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            else
                claims.Add(new Claim(ClaimTypes.Role, "User"));


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.Users)
                {
                    if (item.Username == username && item.Password == password)
                    {
                        if (string.IsNullOrEmpty(returnUrl))
                            returnUrl = "/";

                        var claims = generateClaimsPrincipal(item);
                        await HttpContext.SignInAsync(claims);

                        return Redirect(returnUrl);
                    }
                }

                TempData["Error"] = "Error. Username or password is incorrect. :(";
                return Redirect("login");
            }
        }

        [HttpGet]
        [Route("/[controller]/Logout")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }


        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Personal()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Denied()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult History()
        {
            return View();
        }
    }
}
