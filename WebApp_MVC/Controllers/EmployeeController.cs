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
using System.Text.Json;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

[Authorize]
public class EmployeeController(DataContext dataContext, IWebHostEnvironment webHostEnvironment, IUserService userService, INotificationService notificationService) : BaseController
{
    private readonly IUserService _userService = userService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly DataContext _dataContext = dataContext;
    private readonly INotificationService _notificationService = notificationService;


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var employees = await _userService.GetAllUsers();
        var model = new EmployeeViewModel(_userService);

        if (employees.Success)
        {
            var employeeResult = employees as Result<IEnumerable<ApplicationUser>>;
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
            return ReturnBasedOnRequest(form, "_AddEmployee");
        }

        EmployeeDto employeeDto = form;

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
            employeeDto.ProfileImagePath = "/Images/Uploads/ProfileImages/" + uniqueFileName;
        }

        var result = await _userService.CreateEmployee(employeeDto);

        if (result.Success)
        {
            string notificationMessage = $"New employee {form.FirstName} {form.LastName} added.";
            var notification = NotificationFactory.Create(1, 1, notificationMessage, employeeDto.ProfileImagePath ?? "");
            await _notificationService.AddNotificationAsync(notification);

            return AjaxResult(true, redirectUrl: Url.Action("Index", "Employee"), message: "Employee created.");
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong when creating the employee");
            return ReturnBasedOnRequest(form, "_AddEmployee"); 
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditEmployee(string id)
    {
        var result = await _userService.GetUserById(id);

        if (result.Success)
        {
            var employeeResult = result as Result<ApplicationUser>;
            ApplicationUser employee = employeeResult?.Data ?? new ApplicationUser();

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
            return ReturnBasedOnRequest(model, "_EditEmployee");
        }

        EditEmployeeDto employeeDto = model;

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
            employeeDto.ProfileImagePath = "/Images/Uploads/ProfileImages/" + uniqueFileName;
        }

        ApplicationUser employeeEntity = UserFactory.Create(employeeDto);

        var result = await _userService.UpdateUser(model.Id, employeeEntity);

        if (result.Success)
        {
            return AjaxResult(true, redirectUrl: Url.Action("Index", "Employee"), message: "Team member edited.");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return ReturnBasedOnRequest(model, "_EditEmployee");
        }
    }


    public IActionResult ConfirmDelete(int id)
    {
        return PartialView("_DeleteEmployee", id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployee(string id)
    {
        var result = await _userService.DeleteUser(id);

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

    public async Task<IActionResult> Details(string id)
    {
        var result = await _userService.GetUserById(id);

        if (!result.Success)
        {
            return View("Index");
        }

        var employeeResult = result as Result<ApplicationUser>;
        ApplicationUser employee = employeeResult?.Data ?? new();

        var viewModel = new EmployeeDetailsViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email ?? "Not provided",
            PhoneNumber = employee.PhoneNumber ?? "No phone number has been added",
            JobTitle = employee.JobTitle ?? "Not provided",
            ProfileImagePath = employee.ProfileImagePath,
            DateOfBirth = employee.DateOfBirth ?? default,
            EmployeeProjects = [.. employee.EmployeeProjects],
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

        var result = await _userService.GetUsersBySearchTerm(term);
        if (result.Success)
        {
            var mappedResults = result as IResult<IEnumerable<ApplicationUser>>;
            var employeeList = mappedResults?.Data?.Select(e => new
            {
                Id = e.Id.ToString(),
                MemberFullName = $"{e.FirstName} {e.LastName}",
                ProfileImage = e.ProfileImagePath?.Replace("~", "") ?? "/ProjectImages/Icons/Avatar.svg"
            }).ToList();

            return Json(new { data = employeeList });
        }

        return Json(new List<object>());
    }

    private BadRequestObjectResult JsonValidationError()
    {
        var errors = ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
            );

        return BadRequest(new { success = false, errors });
    }
}
