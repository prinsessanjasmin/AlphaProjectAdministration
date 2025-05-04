using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApp_MVC.Interfaces;

namespace WebApp_MVC.Models;

public class EditProjectViewModel : IProjectViewModel
{
    public int ProjectId { get; set; }

    [Display(Name = "Project Image", Prompt = "Update project image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; } 

    public string? ProjectImagePath { get; set; }

    [Display(Name = "Project Title", Prompt = "Enter project name")]
    [Required(ErrorMessage = "Required")]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "Required")]
    public int ClientId { get; set; } 

    [Display(Name = "Description", Prompt = "Required")]
    public string? Description { get; set; } 

    [Display(Name = "Start Date")]
    [Required(ErrorMessage = "Required")]
    public DateOnly StartDate { get; set; }

    [Display(Name = "End Date")]
    [Required(ErrorMessage = "Required")]
    public DateOnly EndDate { get; set; }

    public decimal? Budget { get; set; }

    [Display(Name = "Status")]
    public int? StatusId { get; set; }
    public List<SelectListItem> StatusOptions { get; set; } = []; 

    public List<SelectListItem> ClientOptions { get; set; } = [];

    public List<TeamMemberDto> PreselectedTeamMembers { get; set; } = [];

    public List<TeamMemberDto> AvailableTeamMembers { get; set; } = [];

    [Display(Name = "Team members", Prompt = "Select team member(s)...")]
    [Required(ErrorMessage = "Required")]
    public string SelectedTeamMemberIds { get; set; } = null!;
    // For handling multiple employee selections Claude AI

    

    public static implicit operator ProjectDto(EditProjectViewModel model)
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
                StatusId = model.StatusId ?? 0,
                SelectedTeamMemberIds = JsonSerializer.Deserialize<List<string>>(model.SelectedTeamMemberIds) ?? []
            }; 
    }

    public EditProjectViewModel(ProjectEntity project)
    {
        ProjectId = project.ProjectId;
        ProjectName = project.ProjectName;
        ClientId = project.ClientId;
        Description = project.Description;
        StartDate = project.StartDate;
        EndDate = project.EndDate;
        Budget = project.Budget;
        StatusId = project.StatusId;
        ProjectImagePath = project.ProjectImagePath;

        var teamMemberIds = project.TeamMembers?.Select(tm => tm.EmployeeId).ToList() ?? [];
        SelectedTeamMemberIds = JsonSerializer.Serialize(teamMemberIds);
    }

    public EditProjectViewModel()
    {
        ClientOptions = [];
    }
}
