using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.DesignationVM;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer")]
    public class DesignationsController : Controller
    {
        private readonly IDesignationService _designationService;

        public DesignationsController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        // GET: Designations
        public async Task<ActionResult> Index()
        {
            var designations = await _designationService.GetAllAsync();
            return View(designations);
        }

        // GET: Designations/Create
        public ActionResult Create()
        {
            var model = new DesignationViewModel
            {
                IsActive = true
            };

            return View(model);
        }

        // POST: Designations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DesignationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _designationService.CreateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Designations/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var designation = await _designationService.GetByIdAsync(id);
            if (designation == null)
            {
                return HttpNotFound();
            }

            return View(designation);
        }

        // POST: Designations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DesignationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _designationService.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Designations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {

            var result = await _designationService.DeleteAsync(id);

            TempData["DeleteMessage"] = result.Message;

            return RedirectToAction("Index");

        }
    }
}