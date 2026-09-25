using HR_PayRoll_Management.DAL.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HR_PayRoll_Management.Services.Service_Shift
{
    public class ShiftService : IShiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShiftService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HR_PayRoll_Management.Models.Shift>> GetAllShiftsAsync()
        {
            return await _unitOfWork.Shifts.GetAllAsync();
        }

        public async Task<HR_PayRoll_Management.Models.Shift> GetShiftByIdAsync(int id)
        {
            return await _unitOfWork.Shifts.GetByIdAsync(id);
        }

        public async Task CreateShiftAsync(HR_PayRoll_Management.Models.Shift shift)
        {
            _unitOfWork.Shifts.Add(shift);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateShiftAsync(HR_PayRoll_Management.Models.Shift shift)
        {
            _unitOfWork.Shifts.Update(shift);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteShiftAsync(int id)
        {
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift != null)
            {
                _unitOfWork.Shifts.Delete(shift);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetShiftDropdownAsync()
        {
            var shifts = await _unitOfWork.Shifts.GetActiveShiftAsync();

            return shifts.Select(x => new SelectListItem
            {
                Value = x.ShiftId.ToString(),
                Text = x.ShiftName
            });
        }
    }
}