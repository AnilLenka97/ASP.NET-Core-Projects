using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers.ViewComponents
{
    public class TotalNumberOfEmployeesViewComponent : ViewComponent
    {
        private readonly IEmployeeRepository employeeRepository;

        public TotalNumberOfEmployeesViewComponent(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public IViewComponentResult Invoke()
        {
            int EmployeeCount = employeeRepository.EmployeeCount();
            return View(EmployeeCount);
        }
    }
}