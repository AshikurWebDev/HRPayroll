using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public interface IEmployeeFileRepository : IRepository<EmployeeFile>
    {
        Task<IEnumerable<EmployeeFile>> GetEmployeeFilesAsync(int employeeId);

    }
}
