
namespace Business.Models;

public class SearchResult
{
    //Had some AI help when creating a free text search. 

    public List<ProjectSearchResult> Projects { get; set; } = []; 
    public List<EmployeeSearchResult> Employees { get; set; } = [];   
    public List<ClientSearchResult> Clients { get; set; } = [];


    public bool HasResults =>
        Projects.Any() || Employees.Any() || Clients.Any();
    public int TotalCount =>
        Employees.Count + Projects.Count + Clients.Count;
}
