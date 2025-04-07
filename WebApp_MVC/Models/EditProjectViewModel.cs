using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebApp_MVC.Interfaces;

namespace WebApp_MVC.Models;

public class EditProjectViewModel : IProjectViewModel
{

    public List<SelectListItem> MemberOptions { get; set; } = new();

    public List<SelectListItem> ClientOptions { get; set; } = new();

    public int ProjectId { get; set; }

    [Display(Name = "Project Image", Prompt = "Update project image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; } 

    public string? ProjectImagePath { get; set; } 

    [Display(Name = "Project Title", Prompt = "Enter project name")]
    [Required(ErrorMessage = "You must name the project.")]
    public string ProjectName { get; set; } 

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
                SelectedTeamMemberIds = model.SelectedTeamMemberIds,
            };
    }

    public EditProjectViewModel()
    {
        // Suggestion from Claude AI 
        MemberOptions = new List<SelectListItem>();
        ClientOptions = new List<SelectListItem>();
        SelectedTeamMemberIds = new List<int>();
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
        SelectedTeamMemberIds = project.TeamMembers.Select(tm => tm.EmployeeId).ToList();
    }
}
