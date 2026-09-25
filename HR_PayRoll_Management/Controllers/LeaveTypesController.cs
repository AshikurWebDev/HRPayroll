using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    [Authorize(Roles = "HR Manager")]
    public class LeaveTypesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: LeaveTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.LeaveTypes.ToListAsync());
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LeaveTypeId,Name,AllowedDays,IsPaid,GenderSpecific,IsActive")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                db.LeaveTypes.Add(leaveType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(leaveType);
        }

        // GET: LeaveTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveType leaveType = await db.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return HttpNotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LeaveTypeId,Name,AllowedDays,IsPaid,GenderSpecific,IsActive")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leaveType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(leaveType);
        }

        // GET: LeaveTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveType leaveType = await db.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return HttpNotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LeaveType leaveType = await db.LeaveTypes.FindAsync(id);
            db.LeaveTypes.Remove(leaveType);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
