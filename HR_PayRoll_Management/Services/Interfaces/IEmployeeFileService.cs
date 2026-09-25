using HR_PayRoll_Management.ViewModels.EmployeeFile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IEmployeeFileService
    {
        Task<IEnumerable<EmployeeFileViewModel>> GetEmployeeFileAsync(int employeeId);
        Task<EmployeeFileViewModel> GetByIdAsync(int id); 
        Task UploadAsync(EmployeeFileUploadViewModel model);
        Task DeleteAsync(int id);
    }
}
