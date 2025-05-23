﻿using Business.Factories;
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
using Microsoft.AspNetCore.Authorization;
using WebApp_MVC.Handlers;
using Microsoft.EntityFrameworkCore;

namespace WebApp_MVC.Controllers;

[Authorize]
public class ProjectController(IClientService clientService, IProjectService projectService, IWebHostEnvironment webHostEnvironment, DataContext dataContext, INotificationService notificationService, IUserService userService, IFileHandler fileHandler, DataContext context) : BaseController
{

    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly DataContext _dataContext = dataContext;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUserService _userService = userService;
    private readonly IFileHandler _fileHandler = fileHandler;
    private readonly DataContext _context = context;


    [HttpGet]
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
        await PopulateStatusesAsync(viewModel);

        ViewData["AddProjectViewModel"] = viewModel;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddProject()
    {
        if (ViewData["AddProjectViewModel"] is AddProjectViewModel viewModel)
        {
            await PopulateClientsAsync(viewModel);
            await PopulateMembersAsync(viewModel);
            await PopulateStatusesAsync(viewModel);
            return PartialView("_AddProject", viewModel);
        }

        // Fallback if ViewData is null - suggestion from Claude AI 
        var fallbackModel = new AddProjectViewModel();
        await PopulateClientsAsync(fallbackModel);
        await PopulateMembersAsync(fallbackModel);
        await PopulateStatusesAsync(fallbackModel);

        return PartialView("_AddProject", fallbackModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(AddProjectViewModel form)
    {
        string originalSelectedTeamMembers = form.SelectedTeamMemberIds;

        await PopulateClientsAsync(form);
        await PopulateMembersAsync(form);
        await PopulateStatusesAsync(form);

        form.SelectedTeamMemberIds = originalSelectedTeamMembers;

        if (!ModelState.IsValid)
        {
            return ReturnBasedOnRequest(form, "_AddProject");
        }

        ProjectDto projectDto = form;

        if (form.ProjectImage != null && form.ProjectImage.Length > 0)
        {
            var imageFileUri = await _fileHandler.UploadFileAsync(form.ProjectImage);
            projectDto.ProjectImagePath = imageFileUri;
        }

        var result = await _projectService.CreateProject(projectDto);

        if (result.Success)
        {
            string notificationMessage = $"New project '{form.ProjectName}' added.";
            var notification = NotificationFactory.Create(1, 2, notificationMessage, projectDto.ProjectImagePath ?? "");
            await _notificationService.AddNotificationAsync(notification);

            return AjaxResult(true, redirectUrl: Url.Action("Index", "Project"), message: "Project created.");
        }

        ModelState.AddModelError("", "Something went wrong when creating the project");
        return ReturnBasedOnRequest(form, "_AddProject");
    }

    [HttpGet]
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
            await PopulateStatusesAsync(viewModel);
            await PopulateMembersAsync(viewModel, id);
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
            return ReturnBasedOnRequest(model, "_EditProject");
        }

        ProjectDto projectDto = model;

        if (model.ProjectImage != null && model.ProjectImage.Length > 0)
        {
            var imageFileUri = await _fileHandler.UploadFileAsync(model.ProjectImage);
            projectDto.ProjectImagePath = imageFileUri;
        }

        ProjectEntity projectEntity = ProjectFactory.Create(projectDto);

        var result = await _projectService.UpdateProject(model.ProjectId, projectEntity);

        if (result.Success)
        {
            string notificationMessage = $"Project '{model.ProjectName}' has been updated.";
            var notification = NotificationFactory.Create(1, 2, notificationMessage, projectDto.ProjectImagePath ?? "");
            Console.WriteLine(notification);
            await _notificationService.AddNotificationAsync(notification);

            return AjaxResult(true, redirectUrl: Url.Action("Index", "Project"), message: "Project edited.");
        }
        else
        {
            ViewBag.ErrorMessage("Something went wrong.");
            return ReturnBasedOnRequest(model, "_EditProject");
        }
    }

    public IActionResult ConfirmDelete(int id)
    {
        // Return just the ID to the partial view
        return PartialView("_DeleteProject", id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var res = await _projectService.GetProjectById(id);

        var projectResult = res as Result<ProjectEntity>;
        ProjectEntity project = projectResult?.Data ?? new ProjectEntity();
        var projectName = project.ProjectName;
        var imagePath = project.ProjectImagePath;


        var result = await _projectService.DeleteProject(id);

        if (result.Success)
        {
            string notificationMessage = $"Project '{projectName}' has been deleted.";
            var notification = NotificationFactory.Create(1, 2, notificationMessage, imagePath ?? "");
            Console.WriteLine(notification);
            await _notificationService.AddNotificationAsync(notification);
            return RedirectToAction("Index", "Project");
        }

        ViewBag.ErrorMessage("Something went wrong.");
        return RedirectToAction("Index", "Project");
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
            Description = project.Description ?? "No description has been added",
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
        var usersResult = await _userService.GetAllUsers();

        if (usersResult is IResult<IEnumerable<ApplicationUser>> typedResult && typedResult.Success && typedResult.Data != null)
        {
            viewModel.AvailableTeamMembers = typedResult.Data.Select(user => new TeamMemberDto
            {
                Id = user.Id,
                MemberFullName = $"{user.FirstName} {user.LastName}",
                ProfileImage = user.ProfileImagePath ?? "Avatar.svg"
            }).ToList();

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
                else
                {
                    viewModel.PreselectedTeamMembers = [];
                    viewModel.SelectedTeamMemberIds = "[]";
                }
            }
            else
            {
                viewModel.PreselectedTeamMembers = [];
                viewModel.SelectedTeamMemberIds = "[]";
            }
        }
        else
        {
            viewModel.AvailableTeamMembers = [];
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

    public async Task PopulateStatusesAsync(IProjectViewModel viewModel)
    {
        var statuses = await _context.Statuses.ToListAsync<StatusEntity>();  

        viewModel.StatusOptions = [];
        foreach (StatusEntity status in statuses)
        {
            viewModel.StatusOptions.Add(new SelectListItem { Text = status.StatusName, Value = status.Id.ToString() });
        }
    }
}
    
    