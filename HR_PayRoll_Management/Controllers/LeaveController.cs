using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.Services.Service_Leave;
using HR_PayRoll_Management.ViewModels.LeaveVM;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;

        public LeaveController(
            ILeaveService leaveService,
            IEmployeeService employeeService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
        }

        [Filters.HasPermission("Leave.ViewOwn")]
        public async Task<ActionResult> Index()
        {
            var user = User.Identity.Name;
            int? employeeId = null;
            bool viewAll = false;
            bool viewTeam = false;

            using (var db = new DAL.AppDbContext())
            {
                var dbUser = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == user);
                if (dbUser != null)
                {
                    employeeId = dbUser.EmployeeId;

                    var role = System.Linq.Queryable.FirstOrDefault(db.Roles, r => r.RoleId == dbUser.RoleId);
                    if (role != null)
                    {
                        if (role.RoleName == "Admin" || role.RoleName == "HR Manager" || role.RoleName == "HR Officer")
                        {
                            viewAll = true;
                        }
                        if (role.RoleName == "Manager")
                        {
                            viewTeam = true;
                        }
                    }
                }
            }

            var leaves = await _leaveService.GetAllAsync(employeeId, viewTeam, viewAll);
            return View(leaves);
        }
        [HttpGet]
        [Filters.HasPermission("Leave.Create")]
        public async Task<ActionResult> Create()
        {
            var model =
                new LeaveCreateViewModel();

            var employees = await _employeeService.GetEmployeeDropdownAsync();
            if (User.IsInRole("Employee") && !User.IsInRole("Admin") && !User.IsInRole("HR Manager"))
            {
                using (var db = new DAL.AppDbContext())
                {
                    var user = System.Linq.Queryable.FirstOrDefault(db.Users.Include("Employee"), u => u.Username == User.Identity.Name);
                    if (user != null && user.Employee != null)
                    {
                        employees = employees.Where(e => e.Value == user.EmployeeId.ToString()).ToList();
                        ViewBag.CurrentEmployeeName = user.Employee.FullName;
                        ViewBag.CurrentEmployeeId = user.Employee.EmployeeId;
                        
                        var year = System.DateTime.Now.Year;
                        var balances = System.Linq.Queryable.Where(db.EmployeeLeaveBalances.Include("LeaveType"), b => b.EmployeeId == user.EmployeeId && b.Year == year).ToList();
                        
                        var allLeaveTypes = System.Linq.Queryable.Where(db.LeaveTypes, lt => lt.IsActive).ToList();
                        // Filter by Gender
                        allLeaveTypes = allLeaveTypes.Where(lt => string.IsNullOrEmpty(lt.GenderSpecific) || lt.GenderSpecific == "A" || lt.GenderSpecific == user.Employee.Gender).ToList();
                        
                        var cardVms = new System.Collections.Generic.List<HR_PayRoll_Management.ViewModels.LeaveVM.LeaveBalanceCardVM>();
                        foreach (var lt in allLeaveTypes)
                        {
                            var bal = balances.FirstOrDefault(b => b.LeaveTypeId == lt.LeaveTypeId);
                            if (bal != null)
                            {
                                cardVms.Add(new HR_PayRoll_Management.ViewModels.LeaveVM.LeaveBalanceCardVM { Name = lt.Name, Allocated = bal.AllocatedDays, Remaining = bal.AllocatedDays - bal.UsedDays });
                            }
                            else
                            {
                                cardVms.Add(new HR_PayRoll_Management.ViewModels.LeaveVM.LeaveBalanceCardVM { Name = lt.Name, Allocated = lt.AllowedDays, Remaining = lt.AllowedDays });
                            }
                        }
                        
                        // Pass to View
                        ViewBag.LeaveBalances = cardVms;
                        
                        // Populate Dropdown for the filtered list
                        model.LeaveTypes = allLeaveTypes.Select(lt => new SelectListItem { Value = lt.LeaveTypeId.ToString(), Text = lt.Name }).ToList();
                    }
                }
            }
            else
            {
                model.LeaveTypes = await _leaveService.GetLeaveTypesAsync();
            }
            ViewBag.Employees = employees;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.HasPermission("Leave.Create")]
        public async Task<ActionResult> Create(LeaveCreateViewModel model)
        {
            var employees = await _employeeService.GetEmployeeDropdownAsync();
            if (User.IsInRole("Employee"))
            {
                using (var db = new DAL.AppDbContext())
                {
                    var user = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == User.Identity.Name);
                    if (user != null)
                    {
                        employees = employees.Where(e => e.Value == user.EmployeeId.ToString()).ToList();

                        // Security check: Employee can only apply for themselves
                        if (model.EmployeeId != user.EmployeeId)
                        {
                            ModelState.AddModelError("", "You can only apply for leave for yourself.");
                        }
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Employees = employees;
                model.LeaveTypes = await _leaveService.GetLeaveTypesAsync();
                return View(model);
            }
            try
            {
                await _leaveService.CreateAsync(model);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Employees = employees;
                model.LeaveTypes = await _leaveService.GetLeaveTypesAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.HasPermission("Leave.CancelOwn")]
        public async Task<ActionResult> Delete(int id)
        {
            await _leaveService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Filters.HasPermission("Leave.Approve")]
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _leaveService.GetByIdAsync(id);
            if (model == null)
                return HttpNotFound();

            ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
            model.LeaveTypes = await _leaveService.GetLeaveTypesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Filters.HasPermission("Leave.Approve")]
        public async Task<ActionResult> Edit(LeaveEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeService.GetEmployeeDropdownAsync();
                model.LeaveTypes = await _leaveService.GetLeaveTypesAsync();

                return View(model);
            }

            await _leaveService.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Filters.HasPermission("Leave.Approve")]
        public async Task<ActionResult> Approve(int id)
        {
            var user = User.Identity.Name;
            using (var db = new DAL.AppDbContext())
            {
                var dbUser = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == user);
                if (dbUser != null)
                {
                    await _leaveService.ApproveLeaveAsync(id, dbUser.UserId);
                }
            }
            return Json(new { success = true, message = "Leave approved successfully." });
        }

        [HttpPost]
        [Filters.HasPermission("Leave.Reject")]
        public async Task<ActionResult> Reject(int id)
        {
            var user = User.Identity.Name;
            using (var db = new DAL.AppDbContext())
            {
                var dbUser = System.Linq.Queryable.FirstOrDefault(db.Users, u => u.Username == user);
                if (dbUser != null)
                {
                    await _leaveService.RejectLeaveAsync(id, dbUser.UserId);
                }
            }
            return Json(new { success = true, message = "Leave rejected successfully." });
        }
    }
}