using System.Collections.Generic;

namespace HR_PayRoll_Management.ViewModels.SalaryVM
{
    public class SalaryStructureMasterVM
    {
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public List<SalaryStructureViewModel> SalaryStructures { get; set; }
        public SalaryStructureMasterVM()
        {
            SalaryStructures = new List<SalaryStructureViewModel>();
        }
        public string SalaryStructuresJson { get; set; }
    }
}