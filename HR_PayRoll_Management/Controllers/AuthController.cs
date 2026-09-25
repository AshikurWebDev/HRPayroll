using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.ViewModels;
using Org.BouncyCastle.Crypto.Generators;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace HR_PayRoll_Management.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private AppDbContext db = new AppDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Include("Role").FirstOrDefault(u => u.Username == model.Username);

                bool isValid = false;
                if (user != null)
                {
                    try
                    {
                        isValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                    }
                    catch
                    {
                        // Fallback if password is not hashed yet (e.g. seeded plain text)
                        isValid = (user.PasswordHash == model.Password);
                    }
                }

                if (isValid)
                {
                    // Create auth ticket
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        user.Username,
                        System.DateTime.Now,
                        System.DateTime.Now.AddMinutes(60),
                        false,
                        user.Role.RoleName
                    );

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    System.Web.HttpCookie faCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auth");
        }
    }
}
