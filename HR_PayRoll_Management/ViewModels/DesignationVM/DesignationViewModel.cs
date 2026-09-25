using System.ComponentModel.DataAnnotations;

namespace HR_PayRoll_Management.ViewModels.DesignationVM
{
    public class DesignationViewModel
    {
        public int DesignationId { get; set; }


        [Required]
        [StringLength(100)]
        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }


        [StringLength(250)]
        public string Description { get; set; }


        public bool IsActive { get; set; }

        public int EmployeeCount { get; set; }
    }
}