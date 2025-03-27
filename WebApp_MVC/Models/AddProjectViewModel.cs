using Azure.Core;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;

namespace WebApp_MVC.Models;


public class AddProjectViewModel(IEmployeeService employeeService, IClientService clientService)
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IClientService _clientService = clientService;

    public List<SelectListItem> MemberOptions { get; private set; } = [];
    public List<SelectListItem> ClientOptions { get; private set; } = [];


    [Display(Name = "Project Image", Prompt = "Upload a project image")]
    public IFormFile? ProjectImage { get; set; }

    [Display(Name = "Project Title", Prompt = "Enter project name")]
    [Required(ErrorMessage = "You must name the project.")]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "You must select a client.")]
    public int ClientId { get; set; }
    public List<ClientSelectListItem>? AvailableClients { get; set; }

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

    public class ClientSelectListItem
    {
        public int ClientId { get; set; }
        public string DisplayName { get; set; } = null!;
        public bool IsSelected { get; set; }
    }

    public static implicit operator ProjectDto(AddProjectViewModel model)
    {
        return model == null
            ? null!
            : new ProjectDto
            {
                ProjectName = model.ProjectName,
                ClientId = model.ClientId,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                SelectedTeamMemberIds = model.SelectedTeamMemberIds,
            };
    }

    public async Task PopulateMemberOptionsAsync()
    {
        var result = await _employeeService.GetAllEmployees();

        if (result.Success)
        {
            var employeeResult = result as Result<IEnumerable<EmployeeEntity>>;
            var employees = employeeResult?.Data;

            MemberOptions.Clear();

            if (employees != null)
            {
                foreach (EmployeeEntity employee in employees)
                {
                    MemberOptions.Add(new SelectListItem()
                    {
                        Value = employee.Id.ToString(),
                        Text = employee.LastName
                    });
                }
            }
        }
    }

    public async Task PopulateClientOptionsAsync()
    {
        var result = await _clientService.GetAllClients();
        
        if (result.Success)
        {
            var clientResult = result as Result<IEnumerable<ClientEntity>>;
            var clients = clientResult?.Data;

            ClientOptions.Clear();

            if (clients != null)
            {
                foreach (ClientEntity client in clients)
                {
                    ClientOptions.Add(new SelectListItem()
                    {
                        Value = client.Id.ToString(),
                        Text = client.ClientName
                    });
                }
            }
        }
    }
    

    public ProjectDto FormData { get; private set; } = new();
    public void ClearFormData()
    {
        FormData = new();
    }
}

