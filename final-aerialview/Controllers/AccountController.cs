using final_aerialview.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DevExpress.Pdf.Native;
using final_aerialview.Utilities;

namespace final_aerialview.Controllers
{
    public class AccountController(DataAccess dataAccess) : Controller
    {
        private readonly DataAccess _dataAccess = dataAccess;

        #region login action, to clear session and remove isloggedin key
        [HttpGet]
        public IActionResult Login()
        {
            // Ensure the user is logged out and session is cleared
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Clear();

            return View();
        }
        #endregion


        #region login action, to validate id and password
        [HttpPost]
        public async Task<IActionResult> Login(string userId, string password)
        {
            var user = await _dataAccess.GetUserByUsernameAsync(userId);
            var projectSettings = _dataAccess.GetPdfImageData();



            //if (user != null && user.Password == password)
                if (user != null && user.Password == password && projectSettings.Any(item => !string.IsNullOrEmpty(item.ModuleKey)))
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

            }else if(user != null && user.Password != password)
            {
            ViewBag.Message = "Invalid credentials";
            }
            else
            {
                ViewBag.Message = "Module Key is not configured. Please contact the admin.";
            }


            return View();
        }
        #endregion

        #region logout action
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("IsLoggedIn");
            HttpContext.Session.Clear(); // Clear session
           
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}
