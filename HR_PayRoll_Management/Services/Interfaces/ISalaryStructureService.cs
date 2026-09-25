
using HR_PayRoll_Management.ViewModels.SalaryVM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface ISalaryStructureService
    {
        Task<IEnumerable<SalaryStructureViewModel>> GetAllAsync();

        Task<SalaryStructureViewModel> GetByIdAsync(int id);

        Task CreateAsync(SalaryStructureViewModel model);

        Task UpdateAsync(
            SalaryStructureViewModel model);

        Task DeleteAsync(int id);
        Task SaveMultipleAsync(List<SalaryStructureViewModel> models);
    }
}
