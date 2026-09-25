using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        // GET: /Notifications/GetUnread
        [HttpGet]
        public JsonResult GetUnread()
        {
            var username = User.Identity.Name;
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null && user.EmployeeId.HasValue)
                {
                    int empId = user.EmployeeId.Value;
                    var cutoff = DateTime.Now.AddDays(-30); // only recent ones
                    var notifs = db.Notifications
                        .Where(n => (n.EmployeeId == empId || n.EmployeeId == null) && !n.IsRead && n.CreatedAt > cutoff)
                        .OrderByDescending(n => n.CreatedAt)
                        .Select(n => new { n.NotificationId, n.Title, n.Message, n.CreatedAt })
                        .ToList();

                    return Json(notifs.Select(n => new {
                        n.NotificationId,
                        n.Title,
                        n.Message,
                        TimeAgo = (DateTime.Now - n.CreatedAt).TotalMinutes < 60 ? Math.Round((DateTime.Now - n.CreatedAt).TotalMinutes) + " mins ago" : Math.Round((DateTime.Now - n.CreatedAt).TotalHours) + " hours ago"
                    }), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new object[0], JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MarkAsRead(int id)
        {
            using (var db = new AppDbContext())
            {
                var n = db.Notifications.Find(id);
                if (n != null)
                {
                    n.IsRead = true;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                var notices = db.Notifications.Where(n => n.EmployeeId == null).OrderByDescending(n => n.CreatedAt).ToList();
                return View(notices);
            }
        }

        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        [HttpPost]
        public ActionResult SendNotice(string Title, string Message)
        {
            using (var db = new AppDbContext())
            {
                var n = new Notification { Title = Title, Message = Message, CreatedAt = DateTime.Now, IsRead = false, EmployeeId = null };
                db.Notifications.Add(n);
                db.SaveChanges();
            }
            TempData["Success"] = "Notice sent to all employees.";
            return RedirectToAction("Index");
        }
    }
}
