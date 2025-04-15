using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Business.Models;

public class ProjectSearchResult
{
    public int Id { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectDescription { get; set; } 
    public string? ClientName { get; set; }
    public string DisplayProperty
    {
        get { return $"{ProjectName}"; }
    }
    public string DetailsUrl
    {
        get { return $"/project/details/{Id}"; }
    }
}
