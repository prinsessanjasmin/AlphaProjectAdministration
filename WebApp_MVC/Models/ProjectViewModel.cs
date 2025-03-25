using Business.Models;
using Business.Services;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class ProjectViewModel(IEmployeeService employeeService, IClientService clientService, IProjectService projectService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;

    [Display(Name = "Project Image", Prompt = "Upload a project image")]
    public IFormFile? ProjectImage { get; set; }

    [Display(Name = "Project Title", Prompt = "Enter project name")]
    [Required(ErrorMessage = "You must name the project.")]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "You must select a client.")]
    public int ClientId { get; set; }

    [Display(Name = "Description", Prompt = "Describe the aims of the project.")]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [Required(ErrorMessage = "You need to select a start date.")]
    public DateOnly StartDate { get; set; }

    [Display(Name = "End Date")]
    [Required(ErrorMessage = "You need to select an end date.")]
    public DateOnly EndDate { get; set; }

    public decimal? Budget { get; set; }

    [Display(Name = "Team members", Prompt = "Select one or more team members")]
    [Required(ErrorMessage = "You must select at least one team member.")]
    public List<int> SelectedTeamMemberIds { get; set; } = [];
    // For handling multiple employee selections Claude AI

    // Optional: Include this if you need to populate a dropdown in the UI
    public List<EmployeeSelectListItem>? AvailableEmployees { get; set; }

    // Helper class for dropdown
    public class EmployeeSelectListItem
    {
        public int EmployeeId { get; set; }
        public string DisplayName { get; set; } = null!;
        public bool IsSelected { get; set; }
    }

    public static implicit operator ProjectDto(ProjectViewModel model)
    {
        return model == null
            ? null!
            : new ProjectDto
            {
                ProjectImage = model.ProjectImage,
                ProjectName = model.ProjectName,
                ClientId = model.ClientId,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                SelectedTeamMemberIds = model.SelectedTeamMemberIds,
            };
    }




    public ProjectDto FormData { get; private set; } = new();
    public List<SelectListItem> MemberOptions { get; private set; } = [];
    public List<SelectListItem> ClientOptions { get; private set; } = [];

    public List<ProjectEntity> Projects { get; private set; } = new();
    public List<EmployeeEntity> Employees { get; private set; } = new();
    public List<ClientEntity> Clients { get; private set; } = new();

    public async Task GetProjects()
    {
        var result = await _projectService.GetAllProjects();

        if (result.Success && result is Result<IEnumerable<ProjectEntity>> dataResult)
        {
            Projects = dataResult.Data.ToList();
        }
        else
        {
            Projects.Clear();
        }
    }

    public async Task PopulateMemberOptionsAsync()
    {
        var result = await _employeeService.GetAllEmployees();

        if (result.Success && result is Result<IEnumerable<EmployeeEntity>> dataResult)
        {
            Employees = dataResult.Data.ToList();
            if (Employees != null)
            {
                foreach (EmployeeEntity employee in Employees)
                {
                    MemberOptions.Add(new SelectListItem()
                    {
                        Value = employee.Id.ToString(),
                        Text = employee.LastName
                    });
                }
            }
            else
            {
                Employees.Clear();
            }
        }

    }

    public async Task PopulateClientOptionsAsync()
    {
        var result = await _clientService.GetAllClients();

        if (result.Success && result is Result<IEnumerable<ClientEntity>> dataResult)
        {
            Clients  = dataResult.Data.ToList();
            if (Clients != null)
            {
                foreach (ClientEntity client in Clients)
                {
                    ClientOptions.Add(new SelectListItem()
                    {
                        Value = client.Id.ToString(),
                        Text = client.ClientName
                    });
                }
            }
            else
            {
                Clients.Clear();
            }
        }
    }

    public void ClearFormData()
    {
        FormData = new();
    }
}
