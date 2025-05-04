using Business.Models;
using WebApp_MVC.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WebApp_MVC.Models;

public class AddProjectViewModel() : IProjectViewModel
{
    [Display(Name = "Project Image", Prompt = "Upload a project image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }

    [Display(Name = "Project Title", Prompt = "Enter project name")]
    [Required(ErrorMessage = "Required")]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "Required")]
    public int ClientId { get; set; }

    [Display(Name = "Description", Prompt = "Describe the aims of the project.")]
    public string? Description { get; set; }

    [Display(Name = "Start Date")]
    [Required(ErrorMessage = "Required")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "End Date")]
    [Required(ErrorMessage = "Required")]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public decimal? Budget { get; set; }

    [Display(Name = "Status")]
    public int? StatusId { get; set; }
    public List<SelectListItem> StatusOptions { get; set; } = [];
    public List<SelectListItem> ClientOptions { get; set; } = [];
    public List<TeamMemberDto> AvailableTeamMembers { get; set; } = [];
    public List<TeamMemberDto> PreselectedTeamMembers { get; set; } = [];

    [Display(Name = "Team members", Prompt = "Select team member(s)...")]
    [Required(ErrorMessage = "Required")]
    
    public string SelectedTeamMemberIds { get; set; } = null!;
    // For handling multiple employee selections Claude AI

    
  


    public static implicit operator ProjectDto(AddProjectViewModel model)
    {
        List<string> teamMembers;
        try
        {
            if (!string.IsNullOrEmpty(model.SelectedTeamMemberIds))
            {
                teamMembers = JsonSerializer.Deserialize<List<string>>(model.SelectedTeamMemberIds) ?? [];
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing team members: {ex.Message}");
            // Log the error, but continue with an empty list
        }


        var projectDto = new ProjectDto
        {
            ProjectName = model.ProjectName,
            ClientId = model.ClientId,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            StatusId = model.StatusId ?? 1,
            SelectedTeamMemberIds = JsonSerializer.Deserialize<List<string>>(model.SelectedTeamMemberIds) ?? []
        }; 

        return projectDto;    
    }
}

