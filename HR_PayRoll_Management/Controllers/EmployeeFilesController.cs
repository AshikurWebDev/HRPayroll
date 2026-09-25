using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.Services.Service_Shift;
using HR_PayRoll_Management.ViewModels.EmployeeFile;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Controllers
{
    public class EmployeeFilesController : Controller
    {
        private readonly IEmployeeFileService _employeeFileService;

        public EmployeeFilesController(
            IEmployeeFileService employeeFileService, IShiftService shiftService)
        {
            _employeeFileService = employeeFileService;
        }



        // GET: EmployeeFiles
        public async Task<ActionResult> Index(int employeeId)
        {
            var files =
                await _employeeFileService.GetEmployeeFileAsync(employeeId);


            ViewBag.EmployeeId = employeeId;


            return View(files);
        }





        // Upload Document
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(EmployeeFileUploadViewModel model)
        {
            try
            {
                if (model.File == null)
                {
                    TempData["Error"] =
                        "Please select a file.";

                    return RedirectToAction(
                        "Index",
                        new { employeeId = model.EmployeeId }
                    );
                }



                await _employeeFileService.UploadAsync(model);



                TempData["Success"] =
                    "File uploaded successfully.";

            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }



            return RedirectToAction(
                "Index",
                new { employeeId = model.EmployeeId }
            );
        }





        // Download Document
        public async Task<ActionResult> Download(int id)
        {
            var file =
                await _employeeFileService.GetByIdAsync(id);



            if (file == null)
            {
                return HttpNotFound();
            }



            string physicalPath =
                Server.MapPath(file.FilePath);



            return File(
                physicalPath,
                file.FileType,
                file.FileName
            );
        }





        // Delete Document
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(
            int id,
            int employeeId)
        {
            try
            {
                await _employeeFileService.DeleteAsync(id);


                TempData["Success"] =
                    "File deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }



            return RedirectToAction(
                "Index",
                new { employeeId = employeeId }
            );
        }
    }
}