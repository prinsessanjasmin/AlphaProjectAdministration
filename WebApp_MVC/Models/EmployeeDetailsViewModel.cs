using Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class EmployeeDetailsViewModel
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string? ProfileImagePath { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public List<ProjectEmployeeEntity> EmployeeProjects { get; set; } = [];
    public string StreetAddress { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
