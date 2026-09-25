using HR_PayRoll_Management.DAL;
using System.Linq;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager")]
    public class AuditLogsController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                var logs = db.AuditLogs.OrderByDescending(x => x.ActionDate).Take(200).ToList();
                return View(logs);
            }
        }
    }
}
