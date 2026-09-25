using HR_PayRoll_Management.ViewModels.DepartmentVM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentViewModel>> GetDepartmentsAsync();
        Task<DepartmentViewModel> GetDepartmentByIdAsync(int id);
        Task CreateAsync(DepartmentViewModel department);
        Task UpdateAsync(DepartmentViewModel department);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<IEnumerable<SelectListItem>> GetDepartmentDropdownAsync();
    }
}
