using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Shift
{
    public interface IShiftService
    {
        Task<IEnumerable<HR_PayRoll_Management.Models.Shift>> GetAllShiftsAsync();
        Task<HR_PayRoll_Management.Models.Shift> GetShiftByIdAsync(int id);
        Task CreateShiftAsync(HR_PayRoll_Management.Models.Shift shift);
        Task UpdateShiftAsync(HR_PayRoll_Management.Models.Shift shift);
        Task DeleteShiftAsync(int id);
        Task<IEnumerable<SelectListItem>> GetShiftDropdownAsync();
    }
}
