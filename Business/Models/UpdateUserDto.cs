namespace Business.Models;

public class UpdateUserDto
{
    public string Id { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string JobTitle { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ProfileImagePath { get; set; }
}
