using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Models; 

public class ProjectFormModel
{
    public IFormFile? ProjectImage { get; set; }
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Select a Client", Prompt = "Select a client")]
    [Required(ErrorMessage = "You must select a client.")]
    public int ClientId { get; set; }
    public string Description { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    [Display(Name = "Team members", Prompt = "Select a team member")]
    [Required(ErrorMessage = "You must select a team member.")]
    public int MemberId { get; set; } 
    public decimal Budget { get; set; }

}
