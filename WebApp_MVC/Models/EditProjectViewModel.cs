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
    public string ProjectName { get; set; } 

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

    public List<SelectListItem> ClientOptions { get; set; } = new();

    [Display(Name = "Team members", Prompt = "Select team member(s)...")]
    [Required(ErrorMessage = "Required")]
    public string SelectedTeamMemberIds { get; set; } = null!;
    // For handling multiple employee selections Claude AI

    public List<TeamMemberDto> PreselectedTeamMembers { get; set; } = [];

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
                StatusId = 1,
                SelectedTeamMemberIds = string.IsNullOrEmpty(model.SelectedTeamMemberIds)
                    ? []
                    : JsonSerializer.Deserialize<List<string>>(model.SelectedTeamMemberIds)
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
        ProjectImagePath = project.ProjectImagePath;

        var teamMemberIds = project.TeamMembers?.Select(tm => tm.EmployeeId).ToList() ?? new List<string>();
        SelectedTeamMemberIds = JsonSerializer.Serialize(teamMemberIds);
    }

    public EditProjectViewModel()
    {
        // Suggestion from Claude AI 
        ClientOptions = new List<SelectListItem>();

    }
}
