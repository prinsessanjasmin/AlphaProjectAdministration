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


public class AddProjectViewModel()
{
    public List<SelectListItem> MemberOptions { get; set; } = new();
    
    public List<SelectListItem> ClientOptions { get; set; } = new();


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
                StatusId = 1,
                SelectedTeamMemberIds = model.SelectedTeamMemberIds,
            };
    }
}

