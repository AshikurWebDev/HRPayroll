using HR_PayRoll_Management.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
