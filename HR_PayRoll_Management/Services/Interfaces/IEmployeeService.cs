using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeViewModel>> GetEmployeesAsync();
        Task<EmployeeViewModel> GetEmployeeDetailsAsync(int id);
        Task<EmployeeEditViewModel> GetEmployeeForEditAsync(int id);
        Task<EmployeeDeleteViewModel> GetEmployeeForDeleteAsync(int id);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task<IEnumerable<SelectListItem>> GetEmployeeDropdownAsync();
        Task<IEnumerable<EmployeeDropdownViewModel>> GetEmployeesByDepartmentAsync(int? departmentId);
        Task<IEnumerable<EmployeeDropdownViewModel>> GetEmployeesByDepartmentDesignationAsync(int departmentId, int designationId);

    }
}