using HR_PayRoll_Management.Helpers;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        public ActionResult Index()
        {
            var settings = SettingsHelper.GetSettings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SystemSettings model, HttpPostedFileBase logoFile)
        {
            var settings = SettingsHelper.GetSettings();
            settings.CompanyName = model.CompanyName;

            if (logoFile != null && logoFile.ContentLength > 0)
            {
                var dir = Server.MapPath("~/Content/Images/");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var fileName = "company_logo" + Path.GetExtension(logoFile.FileName);
                logoFile.SaveAs(Path.Combine(dir, fileName));
                settings.LogoPath = "~/Content/Images/" + fileName;
            }

            SettingsHelper.SaveSettings(settings);
            TempData["Success"] = "Settings updated successfully.";
            return RedirectToAction("Index");
        }
    }
}
