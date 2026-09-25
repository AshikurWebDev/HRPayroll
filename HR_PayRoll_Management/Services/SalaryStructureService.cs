using HR_PayRoll_Management.DAL.UnitOfWork;
using HR_PayRoll_Management.Models;
using HR_PayRoll_Management.Services.Interfaces;
using HR_PayRoll_Management.ViewModels.SalaryVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR_PayRoll_Management.Services
{
    public class SalaryStructureService : ISalaryStructureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalaryStructureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SalaryStructureViewModel>> GetAllAsync()
        {
            var salaries = await _unitOfWork.EmployeeSalaryStructures.GetAllAsync(
                x => x.Employee,
                x => x.Employee.Department
            );

            return salaries.Select(x => new SalaryStructureViewModel
            {
                SalaryStructureId = x.SalaryStructureId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                PhotoPath = x.Employee.PhotoPath,
                BasicSalary = x.BasicSalary,
                HouseRent = x.HouseRent,
                MedicalAllowance = x.MedicalAllowance,
                TransportAllowance = x.TransportAllowance,
                BonusAmount = x.BonusAmount,
                TaxPercentage = x.TaxPercentage,
                EffectiveDate = x.EffectiveDate,
                IsActive = x.IsActive
            });
        }

        public async Task<SalaryStructureViewModel> GetByIdAsync(int id)
        {
            var salaries =
                await _unitOfWork.EmployeeSalaryStructures.GetAllAsync(
                    x => x.Employee);


            var salary =
                salaries.FirstOrDefault(
                    x => x.SalaryStructureId == id);


            if (salary == null)
                return null;


            return new SalaryStructureViewModel
            {
                SalaryStructureId = salary.SalaryStructureId,

                EmployeeId = salary.EmployeeId,

                EmployeeName = salary.Employee.FullName,
                PhotoPath = salary.Employee.PhotoPath,

                BasicSalary = salary.BasicSalary,

                HouseRent = salary.HouseRent,

                MedicalAllowance = salary.MedicalAllowance,

                TransportAllowance = salary.TransportAllowance,

                BonusAmount = salary.BonusAmount,

                TaxPercentage = salary.TaxPercentage,

                EffectiveDate = salary.EffectiveDate,

                IsActive = salary.IsActive
            };
        }

        public async Task CreateAsync(SalaryStructureViewModel model)
        {

            var existing = await _unitOfWork.EmployeeSalaryStructures.FindAsync(x => x.EmployeeId == model.EmployeeId && x.IsActive);


            if (existing.Any())
            {
                throw new Exception(
                    "Active salary structure already exists for this employee.");
            }
            var salary = new EmployeeSalaryStructure
            {
                EmployeeId = model.EmployeeId,
                BasicSalary = model.BasicSalary,
                HouseRent = model.HouseRent,
                MedicalAllowance = model.MedicalAllowance,
                TransportAllowance = model.TransportAllowance,
                BonusAmount = model.BonusAmount,
                TaxPercentage = model.TaxPercentage,
                EffectiveDate = model.EffectiveDate,
                IsActive = true
            };

            _unitOfWork.EmployeeSalaryStructures.Add(salary);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(SalaryStructureViewModel model)
        {
            var salary = await _unitOfWork.EmployeeSalaryStructures.GetByIdAsync(model.SalaryStructureId);
            if (salary == null)
                return;

            salary.BasicSalary = model.BasicSalary;
            salary.HouseRent = model.HouseRent;
            salary.MedicalAllowance = model.MedicalAllowance;
            salary.TransportAllowance = model.TransportAllowance;
            salary.BonusAmount = model.BonusAmount;
            salary.TaxPercentage = model.TaxPercentage;
            salary.EffectiveDate = model.EffectiveDate;
            salary.IsActive = model.IsActive;

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var salary = await _unitOfWork.EmployeeSalaryStructures.GetByIdAsync(id);
            if (salary == null)
                return;

            _unitOfWork.EmployeeSalaryStructures.Delete(salary);
            await _unitOfWork.SaveAsync();
        }

        public async Task SaveMultipleAsync(List<SalaryStructureViewModel> models)
        {
            if (models == null || !models.Any())
            {
                return;
            }

            foreach (var model in models)
            {
                var existing = await _unitOfWork.EmployeeSalaryStructures
                    .FindAsync(x => x.EmployeeId == model.EmployeeId && x.IsActive);

                if (existing.Any())
                {
                    continue;
                }

                var salary = new EmployeeSalaryStructure
                {
                    EmployeeId = model.EmployeeId,
                    BasicSalary = model.BasicSalary,
                    HouseRent = model.HouseRent,
                    MedicalAllowance = model.MedicalAllowance,
                    TransportAllowance = model.TransportAllowance,
                    BonusAmount = model.BonusAmount,
                    TaxPercentage = model.TaxPercentage,
                    EffectiveDate = model.EffectiveDate,
                    IsActive = true
                };

                _unitOfWork.EmployeeSalaryStructures.Add(salary);
            }

            await _unitOfWork.SaveAsync();
        }
    }
}