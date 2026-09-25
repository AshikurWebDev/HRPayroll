using HR_PayRoll_Management.ViewModels.DesignationVM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IDesignationService
    {
        Task<IEnumerable<DesignationViewModel>> GetAllAsync();

        Task<DesignationViewModel> GetByIdAsync(int id);

        Task CreateAsync(DesignationViewModel model);

        Task UpdateAsync(DesignationViewModel model);

        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<IEnumerable<SelectListItem>> GetDesignationDropdownAsync();
        Task<IEnumerable<SelectListItem>>GetDesignationsByDepartmentAsync(int departmentId);
    }
}