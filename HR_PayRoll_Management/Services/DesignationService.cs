using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.DesignationVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesignationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DesignationViewModel>> GetAllAsync()
        {
            var data = await _unitOfWork.Designations.GetAllAsync(x => x.Employees);

            return data.Select(x => new DesignationViewModel
            {
                DesignationId = x.DesignationId,
                DesignationName = x.DesignationName,
                Description = x.Description,
                IsActive = x.IsActive,
                EmployeeCount = x.Employees?.Count ?? 0
            });
        }

        public async Task<DesignationViewModel> GetByIdAsync(int id)
        {
            var designation = await _unitOfWork.Designations.GetByIdAsync(id);
            if (designation == null)
            {
                return null;
            }

            return new DesignationViewModel
            {
                DesignationId = designation.DesignationId,
                DesignationName = designation.DesignationName,
                Description = designation.Description,
                IsActive = designation.IsActive
            };
        }

        public async Task CreateAsync(DesignationViewModel model)
        {
            var existing = await _unitOfWork.Designations
                .FindAsync(x => x.DesignationName.Equals(model.DesignationName, StringComparison.OrdinalIgnoreCase));

            if (existing.Any())
            {
                throw new InvalidOperationException("Designation already exists.");
            }

            var designation = new Designation
            {
                DesignationName = model.DesignationName,
                Description = model.Description,
                IsActive = model.IsActive
            };

            _unitOfWork.Designations.Add(designation);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(DesignationViewModel model)
        {
            var designation = await _unitOfWork.Designations.GetByIdAsync(model.DesignationId);
            if (designation == null)
            {
                return;
            }

            designation.DesignationName = model.DesignationName;
            designation.Description = model.Description;
            designation.IsActive = model.IsActive;

            await _unitOfWork.SaveAsync();
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var employees = await _unitOfWork.Employees
                .FindAsync(x => x.DesignationId == id);

            if (employees.Any())
            {
                return
                (
                    false,
                    $"Cannot delete designation because {employees.Count()} employee(s) are assigned to this designation."
                );
            }

            var designation = await _unitOfWork.Designations.GetByIdAsync(id);
            if (designation == null)
            {
                return
                (
                    false,
                    "Designation not found."
                );
            }

            _unitOfWork.Designations.Delete(designation);
            await _unitOfWork.SaveAsync();

            return
            (
                true,
                "Designation deleted successfully."
            );
        }

        public async Task<IEnumerable<SelectListItem>> GetDesignationDropdownAsync()
        {
            var designations = await _unitOfWork.Designations
                .FindAsync(x => x.IsActive);

            return designations
                .OrderBy(x => x.DesignationName)
                .Select(x => new SelectListItem
                {
                    Value = x.DesignationId.ToString(),
                    Text = x.DesignationName
                });
        }

        public async Task<IEnumerable<SelectListItem>> GetDesignationsByDepartmentAsync(int departmentId)
        {
            // Retrieves active designations of employees present within the specified department
            var employeeDesignationIds = (await _unitOfWork.Employees
                .FindAsync(x => x.DepartmentId == departmentId && x.IsActive))
                .Select(x => x.DesignationId)
                .Distinct()
                .ToList();

            var designations = await _unitOfWork.Designations
                .FindAsync(x => x.IsActive && employeeDesignationIds.Contains(x.DesignationId));

            return designations
                .OrderBy(x => x.DesignationName)
                .Select(x => new SelectListItem
                {
                    Value = x.DesignationId.ToString(),
                    Text = x.DesignationName
                });
        }
    }
}