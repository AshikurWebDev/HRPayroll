using HR_PayRoll_Management.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRespository
    {
        public EmployeeRepository(AppDbContext context) : base(context) { }

        public async Task<Employee> GetEmployeeDetailsAsync(int id)
        {
            return await _context.Employees.Include(x => x.Department).FirstOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<Employee> GetEmployeeWithShiftAsync(int id)
        {
            return await _context.Employees.Include(x => x.Shift).FirstOrDefaultAsync(x => x.EmployeeId == id);
        }

        //public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        //{
        //    return await _context.Employees
        //        .Where(x => x.DepartmentId == departmentId)
        //        .Include(x => x.Department)
        //        .OrderBy(x => x.FullName)
        //        .ToListAsync();
        //}
    }
}