namespace Business.Models;

public class EmployeeSearchResult
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? JobTitle { get; set; }
    public string DisplayProperty
    {
        get { return $"{FirstName} {LastName} - {JobTitle}"; }
    }
    
    public string DetailsUrl
    {
        get { return $"/member/details/{Id}"; }
    }
}
