using HR_PayRoll_Management.ViewModels.LeaveVM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Leave
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveViewModel>> GetAllAsync(int? employeeId = null, bool viewTeam = false, bool viewAll = false);
        Task CreateAsync(LeaveCreateViewModel model);
        Task DeleteAsync(int id);
        Task<IEnumerable<SelectListItem>> GetLeaveTypesAsync();
        Task<LeaveEditViewModel> GetByIdAsync(int id);
        Task UpdateAsync(LeaveEditViewModel model);
        Task ApproveLeaveAsync(int leaveId, int approvedByUserId);
        Task RejectLeaveAsync(int leaveId, int rejectedByUserId);
    }
}
