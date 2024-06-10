using final_aerialview.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace final_aerialview.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataAccess _dataAccess;

        public AccountController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Ensure the user is logged out and session is cleared
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Clear();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userId, string password)
        {
            var user = await _dataAccess.GetUserByUsernameAsync(userId);
            if (user != null && user.Password == password)
            {

                // Determine the role based on the user ID
                string role = user.UserId ?? string.Empty;
                var username =  user.UserName ?? string.Empty;

                var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, username),
                        new(ClaimTypes.Role, role)
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.SetString("IsLoggedIn", "true");

                return RedirectToAction("Index", "Home");

            }

            ViewBag.Message = "Invalid credentials";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("IsLoggedIn");
            return RedirectToAction("Login", "Account");
            //this is simple logout 
        }
    }
}
