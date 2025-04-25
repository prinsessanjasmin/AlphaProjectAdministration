using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Text.Json;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

[Authorize]
public class EmployeeController(DataContext dataContext, IWebHostEnvironment webHostEnvironment, IEmployeeService employeeService) : Controller
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly DataContext _dataContext = dataContext;

    [HttpGet]
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
    public async Task<IActionResult> AddEmployee()
    {
        if (ViewData["AddEmployeeViewModel"] is AddEmployeeViewModel viewModel)
        {
            return PartialView("_AddEmployee", viewModel);
        }

        // Fallback if ViewData is null
        var fallbackModel = new AddEmployeeViewModel();

        return PartialView("_AddEmployee", fallbackModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee(AddEmployeeViewModel form)
    {
        if (!ModelState.IsValid)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                .ToList()
                );

                return BadRequest(new { success = false, errors });
            }
            else
            {
                return View("_AddEmployee", form);
            }
        }

        MemberDto memberDto = form;

        if (form.ProfileImage != null && form.ProfileImage.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + form.ProfileImage.FileName;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Uploads", "ProjectImages");

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Save the file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await form.ProfileImage.CopyToAsync(fileStream);
            }

            // Set the image path in the DTO
            memberDto.ProfileImagePath = "/Images/Uploads/ProfileImages/" + uniqueFileName;
        }

        var result = await _employeeService.CreateEmployee(memberDto);
        var employeeResult = result as Result<EmployeeEntity>;
        EmployeeEntity employee = employeeResult?.Data ?? new EmployeeEntity();

        

        if (result.Success)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok(new { success = true, message = "Team member created successfully" });
            }
            else
            {
                return RedirectToAction("Index", "Employee");
            }
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong when creating the team member");
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return BadRequest(new { success = false, message = "Failed to create team member" });
            }
            else
            {
                return PartialView("_AddEmployee", "Employee");
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditEmployee(int id)
    {
        var result = await _employeeService.GetEmployeeById(id);

        if (result.Success)
        {
            var employeeResult = result as Result<EmployeeEntity>;
            EmployeeEntity employee = employeeResult?.Data ?? new EmployeeEntity();

            var viewModel = new EditEmployeeViewModel(employee);

            return PartialView("_EditEmployee", viewModel);
        }
        else
        {
            ViewBag.ErrorMessage("No team member found");
            return RedirectToAction("Index", "Employee");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditEmployee(EditEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                .ToList()
                );

            return PartialView("_EditEmployee", model);
        }

        MemberDto memberDto = model;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Uploads", "ProfileImages");

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Save the file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(fileStream);
            }

            // Set the image path in the DTO
            memberDto.ProfileImagePath = "/Images/Uploads/ProfileImages/" + uniqueFileName;
        }

        EmployeeEntity employeeEntity = EmployeeFactory.Create(memberDto, model.Id);

        var result = await _employeeService.UpdateEmployee(model.Id, employeeEntity);

        if (result.Success)
        {
            return RedirectToAction("Index", "Employee");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_EditEmployee", model);
        }
    }

    public IActionResult ConfirmDelete(int id)
    {
        return PartialView("_DeleteEmployee", id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeService.DeleteEmployee(id);

        if (result.Success)
        {
            return RedirectToAction("Index", "Employee");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return RedirectToAction("Index", "Employee");
        }
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

            return Json(new { data = employeeList });
        }

        return Json(new List<object>());
    }
}
