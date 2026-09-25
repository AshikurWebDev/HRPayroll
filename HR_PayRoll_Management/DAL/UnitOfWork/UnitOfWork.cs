using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.DAL.Repositories;
using HR_PayRoll_Management.Models;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }


        //employee
        private IEmployeeRespository _employees;
        public IEmployeeRespository Employees
        {
            get
            {
                if (_employees == null)
                    _employees = new EmployeeRepository(_context);

                return _employees;
            }
        }


        //Shift
        private IShiftRepository _shifts;
        public IShiftRepository Shifts
        {
            get
            {
                if (_shifts == null)
                    _shifts = new ShiftRepository(_context);

                return _shifts;
            }
        }

        //Department 
        private IDepartmentRepository _departments;
        public IDepartmentRepository Departments
        {
            get
            {
                if (_departments == null)
                    _departments = new DepartmentRepository(_context);

                return _departments;
            }
        }


        // Attendance 
        private IAttendanceRepository _attendances;
        public IAttendanceRepository Attendances
        {
            get
            {
                if (_attendances == null)
                    _attendances =
                        new AttendanceRepository(_context);

                return _attendances;
            }
        }
        //EmployeeFile 
        private IEmployeeFileRepository _employeeFiles;
        public IEmployeeFileRepository EmployeeFiles
        {
            get
            {
                if (_employeeFiles == null)
                    _employeeFiles =
                        new EmployeeFileRepository(_context);

                return _employeeFiles;
            }
        }
        //Designation  
        private IRepository<Designation> _designations;
        public IRepository<Designation> Designations
        {
            get
            {
                if (_designations == null)
                    _designations = new Repository<Designation>(_context);

                return _designations;
            }
        }

        //Leave repository 
        private IRepository<Leave> _leaves;
        public IRepository<Leave> Leaves
        {
            get
            {
                if (_leaves == null)
                    _leaves = new Repository<Leave>(_context);

                return _leaves;
            }

        }
        private IRepository<LeaveType> _leavesTypes;
        public IRepository<LeaveType> LeaveTypes
        {
            get
            {
                if (_leavesTypes == null)
                    _leavesTypes = new Repository<LeaveType>(_context);

                return _leavesTypes;
            }

        }
        //PayrollCycles 
        private IRepository<PayrollCycle> _payrollCycles;
        public IRepository<PayrollCycle> PayrollCycles
        {
            get
            {
                if (_payrollCycles == null)
                    _payrollCycles =
                        new Repository<PayrollCycle>(_context);

                return _payrollCycles;
            }
        }


        //SalarySlip
        private IRepository<SalarySlip> _salarySlips;
        public IRepository<SalarySlip> SalarySlips
        {
            get
            {
                if (_salarySlips == null)
                    _salarySlips = new Repository<SalarySlip>(_context);

                return _salarySlips;
            }
        }


        //Employee Salary Structure
        private IRepository<EmployeeSalaryStructure> _employeeSalaryStructures;
        public IRepository<EmployeeSalaryStructure> EmployeeSalaryStructures
        {
            get
            {
                if (_employeeSalaryStructures == null)
                {
                    _employeeSalaryStructures = new Repository<EmployeeSalaryStructure>(_context);
                }

                return _employeeSalaryStructures;
            }
        }


        //Users
        private IRepository<User> _users;
        public IRepository<User> Users
        {
            get
            {
                if (_users == null)
                    _users = new Repository<User>(_context);
                return _users;
            }
        }

        //Roles
        private IRepository<Role> _roles;
        public IRepository<Role> Roles
        {
            get
            {
                if (_roles == null)
                    _roles = new Repository<Role>(_context);
                return _roles;
            }
        }

        //Saving
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}