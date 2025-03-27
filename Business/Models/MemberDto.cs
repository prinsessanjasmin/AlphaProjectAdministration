
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class MemberDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string JobTitle { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ProfileImagePath { get; set; }
}
