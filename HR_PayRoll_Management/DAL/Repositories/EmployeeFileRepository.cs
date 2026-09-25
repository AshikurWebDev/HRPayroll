using HR_PayRoll_Management.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public class EmployeeFileRepository : Repository<EmployeeFile>, IEmployeeFileRepository
    {
        public EmployeeFileRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<EmployeeFile>> GetEmployeeFilesAsync(int employeeId)
        {
            return await _context.EmployeeFiles
                .Where(x => x.EmployeeId == employeeId)
                .ToListAsync();
        }
    }
}


