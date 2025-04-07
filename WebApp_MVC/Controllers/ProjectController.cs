using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApp_MVC.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
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
            //var errors = ModelState
            //    .Where(x => x.Value?.Errors.Count > 0)
            //    .ToDictionary(
            //    kvp => kvp.Key,
            //    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
            //    .ToList()
            //    );

            //return BadRequest(new { success = false, errors });
            return View(form);
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
    }


    //[Route("edit")]
    public async Task<IActionResult> EditProject(int id)
    {
        var result = await _projectService.GetProjectById(id);

        if (result.Success)
        {
            var projectResult = result as Result<ProjectEntity>;
            ProjectEntity project = projectResult?.Data ?? new ProjectEntity();

            var viewModel = new EditProjectViewModel(project);

            await PopulateMembersAsync(viewModel);
            await PopulateClientsAsync(viewModel);

            return PartialView("_EditProject", viewModel);
        }
        else
        {
            ViewBag.ErrorMessage("No project found");
            return RedirectToAction("Index", "Project");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditProject(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            //var errors = ModelState
            //    .Where(x => x.Value?.Errors.Count > 0)
            //    .ToDictionary(
            //    kvp => kvp.Key,
            //    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
            //    .ToList()
            //    );

            return View(model);
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


        ProjectEntity projectEntity = ProjectFactory.Create(projectDto); 

        var result = await _projectService.UpdateProject(model.ProjectId, projectEntity);

        if (result.Success)
        {
            return RedirectToAction("Index", "Project");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return PartialView("_EditProject", model);
        }
    }

    public IActionResult ConfirmDelete(int id)
    {
        // Return just the ID to the partial view
        return PartialView("_DeleteProject", id);
    }

    [HttpPost]
    //[Route("delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var result = await _projectService.DeleteProject(id);

        if (result.Success)
        {
            return RedirectToAction("Index", "Project");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return RedirectToAction("Index", "Project");
        }
    }

    public async Task PopulateMembersAsync(IProjectViewModel viewModel)
    {
        var result = await _employeeService.GetAllEmployees();
        var employees = new List<EmployeeEntity>();

        if (result.Success)
        {
            var employeeResult = result as Result<IEnumerable<EmployeeEntity>>;
            employees = employeeResult?.Data?.ToList() ?? [];
        }

        viewModel.MemberOptions = [];
        foreach (EmployeeEntity member in employees)
        {
            viewModel.MemberOptions.Add(new SelectListItem { Text = $"{member.FirstName} {member.LastName}", Value = member.Id.ToString() });
        }

    }

    public async Task PopulateClientsAsync(IProjectViewModel viewModel)
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
    
