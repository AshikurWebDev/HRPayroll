using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.Models;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public interface IEmployeeRespository : IRepository<Employee>
    {
        Task<Employee> GetEmployeeDetailsAsync(int id);
        Task<Employee> GetEmployeeWithShiftAsync(int id);
    }
}
