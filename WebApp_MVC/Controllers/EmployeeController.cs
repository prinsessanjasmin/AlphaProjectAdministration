using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

//[Route("teammembers")]
public class EmployeeController(DataContext dataContext, IEmployeeService employeeService) : Controller
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly DataContext _dataContext = dataContext;

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployees();
        var model = new EmployeeViewModel(_employeeService);

        if (employees.Success)
        {
            var employeeResult = employees as Result<IEnumerable<EmployeeEntity>>;
            model.Employees = employeeResult?.Data?.ToList() ?? [];
        }
        else
        {
            model.Employees = [];
        }
        var viewModel = new AddEmployeeViewModel();

        ViewData["AddEmployeeViewModel"] = viewModel;

        return View(model);
    }

    [HttpGet]
    public async Task<JsonResult> SearchMembers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Json(new List<object>());
        }

        var result = await _employeeService.GetEmployeesBySearchTerm(term);
        if (result.Success)
        {
            var mappedResults = result as IResult<IEnumerable<EmployeeEntity>>;
            var employeeList = mappedResults?.Data?.Select(e => new
            {
                Id = e.Id.ToString(),
                MemberFullName = $"{e.FirstName} {e.LastName}",
                ProfileImage = e.ProfileImagePath?.Replace("~", "") ?? "/ProjectImages/Icons/Avatar.svg"
            }).ToList();

            Console.WriteLine($"Found {employeeList.Count} employees for term: {term}");

            return Json(new { data = employeeList } );
        }

        return Json(new List<object>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _employeeService.GetEmployeeById(id);

        if (!result.Success)
        {
            return View("Index");
        }

        var employeeResult = result as Result<EmployeeEntity>;
        EmployeeEntity employee = employeeResult?.Data ?? new();

        var viewModel = new EmployeeDetailsViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber ?? "No phone number has been added",
            JobTitle = employee.JobTitle,
            ProfileImagePath = employee.ProfileImagePath,
            DateOfBirth = employee.DateOfBirth,
            EmployeeProjects = employee.EmployeeProjects.ToList(),
            StreetAddress = employee.Address.StreetAddress,
            PostCode = employee.Address.PostCode,
            City = employee.Address.City,
            DisplayName = employee.FirstName + " " + employee.LastName,
        };

        return View(viewModel);
    }
}
