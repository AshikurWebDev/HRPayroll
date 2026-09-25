using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.ViewModels;

namespace HR_PayRoll_Management.Mapping
{
    public class EmployeeMapper
    {
        public static EmployeeViewModel ToViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,

                FullName = employee.FullName,
                EmailAddress = employee.EmailAddress,
                Phone = employee.Phone,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                DepartmentName = employee.Department != null ? employee.Department.DepartmentName : "-",


                DesignationName =
        employee.Designation != null
        ? employee.Designation.DesignationName
        : "-",


                ShiftName =
        employee.Shift != null
        ? employee.Shift.ShiftName
        : "-",


                PhotoPath = employee.PhotoPath,

                IsActive = employee.IsActive
            };
        }
    }
}