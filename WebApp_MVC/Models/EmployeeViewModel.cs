using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
namespace WebApp_MVC.Models;

public class EmployeeViewModel(IEmployeeService employeeService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    public List<EmployeeEntity> Employees { get; set; } = [];

    public async Task GetEmployees()
    {
        var result = await _employeeService.GetAllEmployees();

        if (result.Success)
        {
            var employeeResult = result as Result<IEnumerable<EmployeeEntity>>;
            var employees = employeeResult?.Data;

            if (employees != null)
            {
                Employees = employees.ToList();
            }
        }
    }
}
