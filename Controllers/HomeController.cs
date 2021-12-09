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

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.Users)
                {
                    if (item.Username == username && item.Password == password)
                    {
                        if (returnUrl == null || returnUrl == string.Empty)
                            returnUrl = "/";

                        var claims = new List<Claim>()
                        {
                            new Claim("username", username),
                            new Claim("userId", item.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, username),
                            new Claim(ClaimTypes.Name, username),
                        };


                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

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
            var a = User.Identity.Name;
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }


        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Personal()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult History(int siteId)
        {
            //List<PingResult> list = null;
            //SiteSettings siteSettings = null;
            //using (var db = new AppDbContext())
            //{
            //    list = db.PingHistory.Where(i => i.SiteId == siteId).ToList();

            //    foreach (var item in db.SiteList)
            //    {
            //        if(item.Id == siteId)
            //            siteSettings = item;
            //    }

            //    if (siteSettings == null || list.Count() < 1)
            //        return BadRequest();
            //}

            //return View(new History(list, siteSettings));

            return View();
        }
    }
}
