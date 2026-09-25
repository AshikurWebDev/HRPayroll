using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Service_Shift;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "Admin,HR Manager,HR Officer")]
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        public async Task<ActionResult> Index()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return View(shifts);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Shift shift, string StartTime, string EndTime)
        {
            if (TimeSpan.TryParse(StartTime, out TimeSpan st)) shift.StartTime = st;
            if (TimeSpan.TryParse(EndTime, out TimeSpan et)) shift.EndTime = et;

            // Remove ModelState errors for StartTime and EndTime since they failed default binding
            ModelState.Remove("StartTime");
            ModelState.Remove("EndTime");

            if (ModelState.IsValid)
            {
                await _shiftService.CreateShiftAsync(shift);
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            if (shift == null) return HttpNotFound();
            return View(shift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Shift shift, string StartTime, string EndTime)
        {
            if (TimeSpan.TryParse(StartTime, out TimeSpan st)) shift.StartTime = st;
            if (TimeSpan.TryParse(EndTime, out TimeSpan et)) shift.EndTime = et;

            // Remove ModelState errors for StartTime and EndTime since they failed default binding
            ModelState.Remove("StartTime");
            ModelState.Remove("EndTime");

            if (ModelState.IsValid)
            {
                await _shiftService.UpdateShiftAsync(shift);
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await _shiftService.DeleteShiftAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
