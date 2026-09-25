using HR_PayRoll_Management.Services.Dashboard;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IDashboardService _dashboardService;


        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Employee") && !User.IsInRole("Admin") && !User.IsInRole("HR Manager"))
            {
                using (var db = new DAL.AppDbContext())
                {
                    var user = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == User.Identity.Name);
                    if (user != null)
                    {
                        return RedirectToAction("Details", "Employees", new { id = user.EmployeeId });
                    }
                }
            }

            var model =
                await _dashboardService
                .GetDashboardAsync();


            return View(model);
        }

    }
}