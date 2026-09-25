using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.DepartmentVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartmentViewModel>> GetDepartmentsAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync(x => x.Employees);

            return departments.Select(x => new DepartmentViewModel
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                Description = x.Description,
                IsActive = x.IsActive,
                EmployeeCount = x.Employees?.Count ?? 0
            });
        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                return null;
            }

            return new DepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                IsActive = department.IsActive
            };
        }

        public async Task CreateAsync(DepartmentViewModel model)
        {
            var department = new Department
            {
                DepartmentName = model.DepartmentName,
                Description = model.Description,
                IsActive = model.IsActive
            };

            _unitOfWork.Departments.Add(department);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(DepartmentViewModel model)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(model.DepartmentId);
            if (department == null)
            {
                return;
            }

            department.DepartmentName = model.DepartmentName;
            department.Description = model.Description;
            department.IsActive = model.IsActive;

            await _unitOfWork.SaveAsync();
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var employees = await _unitOfWork.Employees
                .FindAsync(e => e.DepartmentId == id);

            if (employees.Any())
            {
                return
                (
                    false,
                    $"Cannot delete department. {employees.Count()} employee(s) are assigned to this department."
                );
            }

            var department = await _unitOfWork.Departments
                .GetByIdAsync(id);

            if (department == null)
            {
                return
                (
                    false,
                    "Department not found."
                );
            }

            _unitOfWork.Departments.Delete(department);

            await _unitOfWork.SaveAsync();

            return
            (
                true,
                "Department deleted successfully."
            );
        }

        public async Task<IEnumerable<SelectListItem>> GetDepartmentDropdownAsync()
        {
            var departments = await _unitOfWork.Departments.FindAsync(x => x.IsActive);

            return departments.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentId.ToString()
            });
        }
    }
}