using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.Services.Service_Attandance;
using HR_PayRoll_Management.Services.Service_Shift;
using HR_PayRoll_Management.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IShiftService _shiftService;
        private readonly IAttendanceService _attendanceService;
        private readonly IDesignationService _designationService;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService, IShiftService shiftService, IAttendanceService attendanceService, IDesignationService designationService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _shiftService = shiftService;
            _attendanceService = attendanceService;
            _designationService = designationService;
        }

        // GET: Employees
        [Authorize(Roles = "Admin,HR Manager,HR Officer,Manager")]
        public async Task<ActionResult> Index()
        {
            var employees = await _employeeService.GetEmployeesAsync();

            if (User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("HR Manager") && !User.IsInRole("HR Officer"))
            {
                using (var db = new DAL.AppDbContext())
                {
                    var user = System.Linq.Queryable.FirstOrDefault(db.Users.Include("Employee"), u => u.Username == User.Identity.Name);
                    if (user != null && user.Employee != null)
                    {
                        int mgrDeptId = user.Employee.DepartmentId;
                        employees = employees.Where(e => e.DepartmentName == user.Employee.Department.DepartmentName).ToList();
                    }
                }
            }

            return View(employees);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Create()
        {
            var model = new EmployeeCreateViewModel();

            model.Departments = await _departmentService.GetDepartmentDropdownAsync();
            model.Designations = await _designationService.GetDesignationDropdownAsync();

            model.Shifts = await _shiftService.GetShiftDropdownAsync();

            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Create(EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _departmentService.GetDepartmentDropdownAsync();
                model.Shifts = await _shiftService.GetShiftDropdownAsync();
                model.Designations = await _designationService.GetDesignationDropdownAsync();

                return View(model);
            }

            string photoPath = null;

            if (model.Photo != null)
            {
                string uploadPath = Server.MapPath("~/Content/Images/Employees/");
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string fullPath = System.IO.Path.Combine(uploadPath, fileName);

                using (var compressedStream = HR_PayRoll_Management.Helpers.ImageCompressionHelper.CompressImage(model.Photo))
                {
                    using (var fileStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
                    {
                        compressedStream.CopyTo(fileStream);
                    }
                }
                photoPath = "~/Content/Images/Employees/" + fileName;
            }

            var employee = new Employee
            {
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                DesignationId = model.DesignationId,
                DepartmentId = model.DepartmentId,
                ShiftId = model.ShiftId,
                PhotoPath = photoPath,
                JoiningDate = DateTime.Now,
                IsActive = true
            };

            await _employeeService.CreateAsync(employee);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);

            if (employee == null)
                return HttpNotFound();

            employee.AttendanceHistory = await _attendanceService.GetEmployeeAttendanceHistoryAsync(id);

            return View(employee);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeForEditAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            employee.Departments = await _departmentService.GetDepartmentDropdownAsync();
            employee.Designations = await _designationService.GetDesignationDropdownAsync();

            employee.Shifts = await _shiftService.GetShiftDropdownAsync();

            return View(employee);
        }

        //Edit: Post Method 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Edit(EmployeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _departmentService.GetDepartmentDropdownAsync();

                return View(model);
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeId);

            if (employee == null)
            {
                return HttpNotFound();
            }

            if (model.Photo != null)
            {
                if (!string.IsNullOrEmpty(employee.PhotoPath))
                {
                    string oldPath = Server.MapPath(employee.PhotoPath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                string uploadPath = Server.MapPath("~/Content/Images/Employees/");
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string fullPath = System.IO.Path.Combine(uploadPath, fileName);

                using (var compressedStream = HR_PayRoll_Management.Helpers.ImageCompressionHelper.CompressImage(model.Photo))
                {
                    using (var fileStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
                    {
                        compressedStream.CopyTo(fileStream);
                    }
                }
                employee.PhotoPath = "~/Content/Images/Employees/" + fileName;
            }

            employee.FullName = model.FullName;
            employee.EmailAddress = model.EmailAddress;
            employee.Phone = model.Phone;
            employee.Gender = model.Gender;
            employee.DateOfBirth = model.DateOfBirth;
            employee.DesignationId = model.DesignationId;
            employee.DepartmentId = model.DepartmentId;

            await _employeeService.UpdateAsync(employee);

            return RedirectToAction("Index");
        }

        //Delete: Get 
        [HttpGet]
        [Authorize(Roles = "Admin,HR Manager,HR Officer")]
        public async Task<ActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeForDeleteAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        //Delete: Post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> ConfirmDelete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(employee.PhotoPath))
            {
                string oldPath = Server.MapPath(employee.PhotoPath);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            await _employeeService.DeleteAsync(employee);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<PartialViewResult> FilterEmployeeAttendance(int employeeId, int month, int year)
        {
            var attendanceHistory = await _attendanceService.GetEmployeeAttendanceHistoryByMonthAsync(employeeId, month, year);

            return PartialView(
                "_AttendanceHistory",
                attendanceHistory);
        }
    }
}




