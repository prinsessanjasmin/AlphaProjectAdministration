using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Models; 

public class ProjectDto
{
    public string? ProjectImagePath { get; set; }
    public string ProjectName { get; set; } = null!;
    public int ClientId { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int StatusId { get; set; }
    public decimal? Budget { get; set; }
    public List<string> SelectedTeamMemberIds { get; set; } = [];
}
