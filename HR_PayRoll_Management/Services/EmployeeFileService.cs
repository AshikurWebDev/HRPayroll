
using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Helpers;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.EmployeeFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace HR_PayRoll_Management.Services
{
    public class EmployeeFileService : IEmployeeFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeFileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EmployeeFileViewModel>> GetEmployeeFileAsync(int employeeId)
        {
            var files = await _unitOfWork.EmployeeFiles.GetEmployeeFilesAsync(employeeId);

            return files.Select(x => new EmployeeFileViewModel
            {
                FileId = x.FileId,
                EmployeeId = x.EmployeeId,
                FileName = x.FileName,
                FilePath = x.FilePath,
                FileType = x.FileType,
                UploadDate = x.UploadDate
            });
        }


        //GetById 
        public async Task<EmployeeFileViewModel> GetByIdAsync(int id)
        {
            var file = await _unitOfWork.EmployeeFiles.GetByIdAsync(id);

            if (file == null)
                return null;


            return new EmployeeFileViewModel
            {
                FileId = file.FileId,
                EmployeeId = file.EmployeeId,
                FileName = file.FileName,
                FilePath = file.FilePath,
                FileType = file.FileType,
                UploadDate = file.UploadDate
            };
        }


        public async Task UploadAsync(EmployeeFileUploadViewModel model)
        {
            var filepath = EmployeeFileHelper.Upload(model.File, model.EmployeeId);

            var employee = new EmployeeFile
            {
                EmployeeId = model.EmployeeId,
                FileName = Path.GetFileName(filepath),
                FilePath = filepath,
                FileType = model.File.ContentType,
                UploadDate = DateTime.Now
            };

            _unitOfWork.EmployeeFiles.Add(employee);

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var file =
                await _unitOfWork.EmployeeFiles.GetByIdAsync(id);

            if (file == null)
                return;


            EmployeeFileHelper.Delete(file.FilePath);

            _unitOfWork.EmployeeFiles.Delete(file);

            await _unitOfWork.SaveAsync();
        }
    }
}