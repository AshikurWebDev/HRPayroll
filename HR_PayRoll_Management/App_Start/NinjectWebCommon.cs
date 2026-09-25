using HR_PayRoll_Management.DAL;
using HR_PayRoll_Management.DAL.Interfaces;
using HR_PayRoll_Management.DAL.Repositories;
using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Services;
using HR_PayRoll_Management.Services.Dashboard;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.Services.Service_Attandance;
using HR_PayRoll_Management.Services.Service_Leave;
using HR_PayRoll_Management.Services.Service_Shift;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(HR_PayRoll_Management.App_Start.NinjectWebCommon),
    "Start")]

[assembly: WebActivatorEx.ApplicationShutdownMethod(
    typeof(HR_PayRoll_Management.App_Start.NinjectWebCommon),
    "Stop")]

namespace HR_PayRoll_Management.App_Start
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper =
            new Bootstrapper();


        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(
                typeof(OnePerRequestHttpModule));

            DynamicModuleUtility.RegisterModule(
                typeof(NinjectHttpModule));

            bootstrapper.Initialize(CreateKernel);
        }
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }


        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<AppDbContext>()
                .ToSelf()
                .InRequestScope();

            kernel.Bind(typeof(IRepository<>))
                .To(typeof(Repository<>))
                .InRequestScope();

            kernel.Bind<IUnitOfWork>()
                .To<UnitOfWork>()
                .InRequestScope();

            kernel.Bind<IEmployeeService>()
                .To<EmployeeService>()
                .InRequestScope();

            kernel.Bind<IPayrollService>()
                .To<PayrollService>()
                .InRequestScope();

            kernel.Bind<IDepartmentService>()
                .To<DepartmentService>()
                .InRequestScope();


            //here the EmployeeFile is started 

            kernel.Bind<IEmployeeFileRepository>()
                .To<EmployeeFileRepository>()
                .InRequestScope();

            kernel.Bind<IEmployeeFileService>()
                .To<EmployeeFileService>()
                .InRequestScope();

            //Shift 
            kernel.Bind<IShiftRepository>()
                .To<ShiftRepository>()
                .InRequestScope();

            kernel.Bind<IShiftService>()
                .To<ShiftService>()
                .InRequestScope();

            //Updating the dashboard

            kernel.Bind<IDashboardService>()
                .To<DashboardService>()
                .InRequestScope();

            //Attendance service here 
            kernel.Bind<IAttendanceRepository>()
                .To<AttendanceRepository>()
                .InRequestScope();

            kernel.Bind<IAttendanceService>()
                .To<AttendanceService>()
                .InRequestScope();

            //Leave Servids here 
            kernel.Bind<ILeaveService>()
                .To<LeaveService>()
                .InRequestScope();

            //salary structure here 
            kernel.Bind<ISalaryStructureService>()
                .To<SalaryStructureService>()
                .InRequestScope();

            //Designation structure 
            kernel.Bind<IDesignationService>()
                .To<DesignationService>()
                .InRequestScope();

            //Payroll Reports
            kernel.Bind<IPayrollReportService>()
                .To<PayrollReportService>()
                .InRequestScope();

            //Users
            kernel.Bind<IUserService>()
                .To<UserService>()
                .InRequestScope();
        }
    }
}