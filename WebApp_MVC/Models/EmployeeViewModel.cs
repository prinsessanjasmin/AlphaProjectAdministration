using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
namespace WebApp_MVC.Models;

public class EmployeeViewModel(IUserService userService)
{
    private readonly IUserService _userService = userService;
    public List<ApplicationUser> Employees { get; set; } = [];

    public async Task GetEmployees()
    {
        var result = await _userService.GetAllUsers();

        if (result.Success)
        {
            var employeeResult = result as Result<IEnumerable<ApplicationUser>>;
            var employees = employeeResult?.Data;

            if (employees != null)
            {
                Employees = employees.ToList();
            }
        }
    }
}
