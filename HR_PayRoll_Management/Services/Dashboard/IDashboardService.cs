using HR_PayRoll_Management.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardAsync();
    }
}
