using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers;

//[Route("projects")]
public class ProjectController(IEmployeeService employeeService, IClientService clientService, IProjectService projectService, IWebHostEnvironment webHostEnvironment) : Controller
{

    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;



    //[Route("")]
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
        var viewModel = new AddProjectViewModel();

        await PopulateClientsAsync(viewModel);
        await PopulateMembersAsync(viewModel);

        ViewData["AddProjectViewModel"] = viewModel; 

        return View(model);
    }

    //[Route("add")]
    public async Task<IActionResult> AddProject()
    {
        if (ViewData["AddProjectViewModel"] is AddProjectViewModel viewModel)
        {
            return PartialView("_AddProject", viewModel);
        }

        // Fallback if ViewData is null
        var fallbackModel = new AddProjectViewModel();
        await PopulateClientsAsync(fallbackModel);
        await PopulateMembersAsync(fallbackModel);

        return PartialView("_AddProject", fallbackModel);  
    }



    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel form)
    {
        if (!ModelState.IsValid)
        {
            // Repopulate dropdowns in case of validation errors
            //form.ClientOptions = await PopulateClientsAsync(form);
            //form.MemberOptions = await PopulateMembersAsync(form);
            return PartialView("_AddProject", form);
        }

        ProjectDto projectDto = form;
        

        if (form.ProjectImage != null && form.ProjectImage.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + form.ProjectImage.FileName;
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Uploads", "ProjectImages");

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Save the file
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await form.ProjectImage.CopyToAsync(fileStream);
            }

            // Set the image path in the DTO
            projectDto.ProjectImagePath = "/Images/Uploads/ProjectImages/" + uniqueFileName;
        }

        var result = await _projectService.CreateProject(projectDto);

        if (result.Success)
        {
            return RedirectToAction("Index", "Project");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_AddProject", form);
        }


        return RedirectToAction("Index", "Project");
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

    public async Task PopulateMembersAsync(AddProjectViewModel viewModel)
    {
        var result = await _employeeService.GetAllEmployees();
        var employees = new List<EmployeeEntity>();

        if (result.Success)
        {
            var employeeResult = result as Result<IEnumerable<EmployeeEntity>>;
            employees = employeeResult?.Data?.ToList() ?? [];
        }

        viewModel.MemberOptions = new List<SelectListItem>();
        foreach (EmployeeEntity member in employees)
        {
            viewModel.MemberOptions.Add(new SelectListItem { Text = $"{member.FirstName} {member.LastName}", Value = member.Id.ToString() });
        }

    }

    public async Task PopulateClientsAsync(AddProjectViewModel viewModel)
    {
        var result = await _clientService.GetAllClients();
        var clients = new List<ClientEntity>();

        if (result.Success)
        {
            var clientResult = result as Result<IEnumerable<ClientEntity>>;
            clients = clientResult?.Data?.ToList() ?? [];
        }

        viewModel.ClientOptions = new List<SelectListItem>();
        foreach (ClientEntity client in clients)
        {
            viewModel.ClientOptions.Add(new SelectListItem { Text = client.ClientName, Value = client.Id.ToString() });
        }
    }
}
    
