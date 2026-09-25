using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    }
}
