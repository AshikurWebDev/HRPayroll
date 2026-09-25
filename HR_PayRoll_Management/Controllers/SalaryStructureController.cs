using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.SalaryVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer")]
    public class SalaryStructureController : Controller
    {
        private readonly ISalaryStructureService _salaryService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;

        public SalaryStructureController(
            ISalaryStructureService salaryService, IEmployeeService employeeService, IDepartmentService departmentService, IDesignationService designationService)
        {
            _salaryService = salaryService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _designationService = designationService;

        }

        public async Task<ActionResult> Index()
        {
            var data = await _salaryService.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new SalaryStructureMasterVM();
            ViewBag.Departments = await _departmentService.GetDepartmentDropdownAsync();
            ViewBag.Designations = await _designationService.GetDesignationDropdownAsync();
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
    SalaryStructureMasterVM model)
        {

            if (!string.IsNullOrEmpty(model.SalaryStructuresJson))
            {

                model.SalaryStructures =
                    Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<SalaryStructureViewModel>>
                    (
                        model.SalaryStructuresJson
                    );

            }


            if (model.SalaryStructures == null ||
               !model.SalaryStructures.Any())
            {

                ModelState.AddModelError(
                    "",
                    "Please add at least one employee salary."
                );

            }



            if (!ModelState.IsValid)
            {

                ViewBag.Departments =
                    await _departmentService.GetDepartmentDropdownAsync();


                ViewBag.Designations =
                    await _designationService.GetDesignationDropdownAsync();


                return View(model);

            }



            await _salaryService.SaveMultipleAsync(
                model.SalaryStructures
            );



            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _salaryService.GetByIdAsync(id);
            if (model == null)
                return HttpNotFound();

            ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SalaryStructureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
                return View(model);
            }

            await _salaryService.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await _salaryService.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        // For AJAX endpoint for deisgnation 
        [HttpGet]
        public async Task<JsonResult> GetDesignationsByDepartment(int departmentId)
        {

            var designations =
                await _designationService
                .GetDesignationsByDepartmentAsync(departmentId);


            var result =
                designations.Select(x => new
                {
                    id = x.Value,
                    name = x.Text
                });


            return Json(
                result,
                JsonRequestBehavior.AllowGet
            );

        }

        //For AJAX endpoint for employees 
        [HttpGet]
        public async Task<JsonResult> GetEmployees(
    int departmentId,
    int designationId)
        {

            var employees =
                await _employeeService
                .GetEmployeesByDepartmentDesignationAsync(
                    departmentId,
                    designationId
                );


            var result =
                employees.Select(x => new
                {
                    id = x.EmployeeId,
                    name = x.FullName,
                    image = Url.Content(x.PhotoPath)
                });



            return Json(
                result,
                JsonRequestBehavior.AllowGet
            );

        }
    }
}