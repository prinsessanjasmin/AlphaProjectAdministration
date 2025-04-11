using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApp_MVC.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebApp_MVC.Models;
using Data.Contexts;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApp_MVC.Controllers;

//[Route("projects")]
public class ProjectController(IEmployeeService employeeService, IClientService clientService, IProjectService projectService, IWebHostEnvironment webHostEnvironment, DataContext dataContext) : Controller
{

    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly DataContext _dataContext = dataContext;



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
            await PopulateClientsAsync(viewModel);
            await PopulateMembersAsync(viewModel);
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
        await PopulateClientsAsync(form);
        await PopulateMembersAsync(form);

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
                return View("_AddProject", form);
            } 
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

        if (string.IsNullOrEmpty(form.SelectedTeamMemberIds))
        {
            try
            {
                var memberIds = JsonSerializer.Deserialize<List<int>>(form.SelectedTeamMemberIds);
                projectDto.SelectedTeamMemberIds = memberIds;
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError("SelectedTeamMemberIds", "Invalid team member selection");
                return PartialView("_AddProject", form); 
            }
        }

        var result = await _projectService.CreateProject(projectDto);

        if (result.Success)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Ok(new { success = true, message = "Project created successfully" });
            }
            else
            {
                return RedirectToAction("Index", "Project");
            }
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong when creating the project");
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return BadRequest(new { success = false, message = "Failed to create project" });
            }
            else
            {
                return PartialView("_AddProject", "Project");
            }
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

            if (project.TeamMembers?.Count > 0)
            {
                viewModel.PreselectedTeamMembers = project.TeamMembers.Select(tm => new TeamMemberDto
                {
                    Id = tm.EmployeeId.ToString(),
                    MemberFullName = $"{tm.Employee.FirstName} {tm.Employee.LastName}",
                    ProfileImage = tm.Employee.ProfileImagePath?.Replace("~", "") ?? "Avatar.svg"
                }).ToList();

                var teamMemberIds = project.TeamMembers.Select(tm => tm.EmployeeId).ToList();
                viewModel.SelectedTeamMemberIds = JsonSerializer.Serialize(teamMemberIds);
            }
            else
            {
                viewModel.PreselectedTeamMembers = new List<TeamMemberDto>();
                viewModel.SelectedTeamMemberIds = "[]";
            }

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
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                .ToList()
                );

            return PartialView("_EditProject", model);
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

        if (!string.IsNullOrEmpty(model.SelectedTeamMemberIds))
        {
            try
            {
                var memberIds = JsonSerializer.Deserialize<List<int>>(model.SelectedTeamMemberIds);
                projectDto.SelectedTeamMemberIds = memberIds;
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError("SelectedTeamMemberIds", "Invalid team member selection");
                return PartialView("_EditProject", model);
            }
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

    public async Task<IActionResult> Details(int id)
    {
        var result = await _projectService.GetProjectById(id);

        if (!result.Success)
        {
            return View("Index");
        }
        
        var projectResult = result as Result<ProjectEntity>;
        ProjectEntity project = projectResult?.Data ?? new();

        var viewModel = new ProjectDetailsViewModel
        {
            Id = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectImagePath = project.ProjectImagePath,
            Description = (project.Description ??  "No description has been added"),
            Client = project.Client.ClientName,
            Status = project.Status.StatusName,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            TeamMembers = project.TeamMembers.ToList()
        };  
        
        return View(viewModel);
    }

    public async Task PopulateMembersAsync(IProjectViewModel viewModel, int? projectId = null)
    {
        if (projectId.HasValue)
        {
            var project = await _projectService.GetProjectById(projectId.Value);
            IResult<ProjectEntity>? projectResult = project as IResult<ProjectEntity>;
            if (projectResult != null && projectResult?.Data?.TeamMembers?.Count > 0)
            {
                viewModel.PreselectedTeamMembers = projectResult.Data.TeamMembers.Select(tm => new TeamMemberDto
                {
                    Id = tm.EmployeeId.ToString(),
                    MemberFullName = (tm.Employee.FirstName + ' ' + tm.Employee.LastName).ToString(),
                    ProfileImage = tm.Employee.ProfileImagePath ?? "Avatar.svg"
                }).ToList();

                var teamMemberIds = projectResult.Data.TeamMembers.Select(tm => tm.EmployeeId).ToList();
                viewModel.SelectedTeamMemberIds = JsonSerializer.Serialize(teamMemberIds);
            }
        }
        else
        {
            viewModel.PreselectedTeamMembers = [];
            viewModel.SelectedTeamMemberIds = "[]"; 
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

        viewModel.ClientOptions = [];
        foreach (ClientEntity client in clients)
        {
            viewModel.ClientOptions.Add(new SelectListItem { Text = client.ClientName, Value = client.Id.ToString() });
        }
    }
}
    
