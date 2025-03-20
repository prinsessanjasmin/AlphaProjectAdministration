using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Models; 

public class ProjectFormModel
{
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
    public List<int> SelectedTeamMemberIds { get; set; } = new List<int>();
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
}
