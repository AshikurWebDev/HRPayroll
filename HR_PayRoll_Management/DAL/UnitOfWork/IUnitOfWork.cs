using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.DAL.Repositories;
using HR_PayRoll_Management.Models;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IEmployeeRespository Employees { get; }
        IDepartmentRepository Departments { get; }
        IAttendanceRepository Attendances { get; }
        IEmployeeFileRepository EmployeeFiles { get; }
        IShiftRepository Shifts { get; }
        IRepository<PayrollCycle> PayrollCycles { get; }
        IRepository<SalarySlip> SalarySlips { get; }
        IRepository<Leave> Leaves { get; }
        IRepository<LeaveType> LeaveTypes { get; }
        IRepository<EmployeeSalaryStructure> EmployeeSalaryStructures { get; }
        IRepository<Designation> Designations { get; }
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        Task SaveAsync();

    }
}
