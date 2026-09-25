using HR_PayRoll_Management.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.Repositories
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<Shift>> GetActiveShiftAsync()
        {
            return await _context.Shifts.OrderBy(x => x.ShiftName).ToListAsync();
        }
    }
}