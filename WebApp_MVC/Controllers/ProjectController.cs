using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

//[Route("projects")]
public class ProjectController(IEmployeeService employeeService, IClientService clientService, IProjectService projectService, IWebHostEnvironment webHostEnvironment) : Controller
{

    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllProjects();
        var model = new ProjectViewModel(_projectService);

        if (projects.Success)
        {
            var projectResult = projects as Result<IEnumerable<ProjectEntity>>;
            model.Projects = projectResult?.Data?.ToList() ?? []; 
        }
        else
        {
            model.Projects = [];
        }
        ViewData["AddProjectViewModel"] = new AddProjectViewModel(_employeeService, _clientService);
        return View(model);
    }

    //[Route("add")]
    public async Task<IActionResult> AddProject()
    {
        var viewModel = new AddProjectViewModel(_employeeService, _clientService); 
        await viewModel.PopulateClientOptionsAsync();
        await viewModel.PopulateMemberOptionsAsync();
        return PartialView("_AddProject", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await model.PopulateMemberOptionsAsync();
            await model.PopulateClientOptionsAsync();
            return PartialView("_AddProject", model);
        }

        ProjectDto projectDto = model;

        if (model.ProjectImage != null && model.ProjectImage.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProjectImage.FileName;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Uploads", "ProjectImages");

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Save the file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProjectImage.CopyToAsync(fileStream);
            }

            // Set the image path in the DTO
            projectDto.ProjectImagePath = "/Images/Uploads/ProjectImages/" + uniqueFileName;
        }


        
        var result = await _projectService.CreateProject(projectDto);

        if (result.Success)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_AddProject", model);
        }
    }

    //[Route("edit")]
    public IActionResult EditProject()
    {
        return View();
    }

    //[Route("delete")]
    public IActionResult DeleteProject()
    {
        return View();
    }
}
