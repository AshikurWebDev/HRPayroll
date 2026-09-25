using HR_PayRoll_Management.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AppUser = HR_PayRoll_Management.Models.User;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "HR Manager")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<ActionResult> Search(string query)
        {
            var users = await _userService.GetAllUsersAsync();
            if (!string.IsNullOrEmpty(query))
            {
                users = users.Where(u => u.Username.ToLower().Contains(query.ToLower()));
            }
            return PartialView("_UsersTablePartial", users);
        }

        public async Task<ActionResult> Create()
        {
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AppUser user)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                }
                await _userService.CreateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return HttpNotFound();

            var roles = await _userService.GetAllRolesAsync();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AppUser user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                ModelState.Remove("PasswordHash");
            }

            if (ModelState.IsValid)
            {
                var existing = await _userService.GetUserByIdAsync(user.UserId);
                existing.Username = user.Username;
                existing.RoleId = user.RoleId;
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                }
                await _userService.UpdateUserAsync(existing);
                return RedirectToAction(nameof(Index));
            }
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<ActionResult> MyProfile()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            var users = await _userService.GetAllUsersAsync();
            var currentUser = users.FirstOrDefault(u => u.Username == User.Identity.Name);

            if (currentUser == null) return HttpNotFound();

            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateProfileImage(System.Web.HttpPostedFileBase profileImage)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            var users = await _userService.GetAllUsersAsync();
            var currentUser = users.FirstOrDefault(u => u.Username == User.Identity.Name);

            if (currentUser != null && currentUser.Employee != null && profileImage != null)
            {
                // We use ImageCompressionHelper to compress the image
                string uploadPath = Server.MapPath("~/Content/Images/Employees/");
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }

                // Make sure to use .jpg since the helper formats it as Jpeg
                string fileName = HR_PayRoll_Management.Helpers.EmployeeFileHelper.GenerateFileName(currentUser.EmployeeId.Value, ".jpg");
                string fullPath = System.IO.Path.Combine(uploadPath, fileName);

                // Compress image using helper
                using (var compressedStream = HR_PayRoll_Management.Helpers.ImageCompressionHelper.CompressImage(profileImage))
                {
                    // Save to disk
                    using (var fileStream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
                    {
                        compressedStream.CopyTo(fileStream);
                    }
                }

                currentUser.Employee.PhotoPath = "~/Content/Images/Employees/" + fileName;

                // Assuming we can update the Employee through UnitOfWork
                var db = new HR_PayRoll_Management.DAL.AppDbContext();
                var emp = await db.Employees.FindAsync(currentUser.EmployeeId.Value);
                if (emp != null)
                {
                    emp.PhotoPath = currentUser.Employee.PhotoPath;
                    await db.SaveChangesAsync();
                }
            }

            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateProfileInfo(int EmployeeId, string FullName, string Phone, System.DateTime DateOfBirth)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            using (var db = new HR_PayRoll_Management.DAL.AppDbContext())
            {
                var user = System.Linq.Queryable.FirstOrDefault(db.Users.Include("Employee"), u => u.Username == User.Identity.Name);
                if (user != null && user.Employee != null && user.EmployeeId == EmployeeId)
                {
                    user.Employee.FullName = FullName;
                    user.Employee.Phone = Phone;
                    user.Employee.DateOfBirth = DateOfBirth;
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}

