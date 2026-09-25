using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.DepartmentVM;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<ActionResult> Index()
        {
            var departments = await _departmentService.GetDepartmentsAsync();
            return View(departments);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _departmentService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        // GET: Departments/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _departmentService.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteAsync(id);

            TempData["Message"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}