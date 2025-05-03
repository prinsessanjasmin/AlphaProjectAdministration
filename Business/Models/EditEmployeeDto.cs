namespace Business.Models;
public class EditEmployeeDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string JobTitle { get; set; } = null!;
    public int? AddressId { get; set; } 
    public string StreetAddress { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ProfileImagePath { get; set; }
    public string Role { get; set; } = null!;
}
