using Business.Models;
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class EditEmployeeViewModel
{
    public int Id { get; set; }

    [Display(Name = "First Name", Prompt = "Enter first name")]
    [Required(ErrorMessage = "Required")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter last name")]
    [Required(ErrorMessage = "Required")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Job Title", Prompt = "Enter job title")]
    [Required(ErrorMessage = "Required")]
    public string JobTitle { get; set; } = null!;

    [Display(Name = "Project Image", Prompt = "Upload a project image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProfileImage { get; set; }
    public string? ProfileImagePath { get; set; }

    [Display(Name = "Day")]
    [Required(ErrorMessage = "Required")]
    public int Day { get; set; }

    [Display(Name = "Month")]
    [Required(ErrorMessage = "Required")]
    public int Month { get; set; }

    [Display(Name = "Year")]
    [Required(ErrorMessage = "Required")]
    public int Year { get; set; }

    [Display(Name = "Street Address", Prompt = "Enter street address")]
    [Required(ErrorMessage = "Required")]
    public string StreetAddress { get; set; } = null!;

    [Display(Name = "Post Code", Prompt = "Enter post code")]
    [Required(ErrorMessage = "Required")]
    public string PostCode { get; set; } = null!;

    [Display(Name = "City", Prompt = "Enter city")]
    [Required(ErrorMessage = "Required")]
    public string City { get; set; } = null!;

    public static implicit operator MemberDto(EditEmployeeViewModel model)
    {
        return model == null
            ? null!
            : new MemberDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                ProfileImagePath = model.ProfileImagePath,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                StreetAddress = model.StreetAddress,
                PostCode = model.PostCode,
                City = model.City,
                DateOfBirth = DateConverter(model.Day, model.Month, model.Year),
            };
    }

    public EditEmployeeViewModel(EmployeeEntity entity)
    {
        Id = entity.Id;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
        Email = entity.Email;
        PhoneNumber = entity.PhoneNumber;
        JobTitle = entity.JobTitle;
        ProfileImagePath = entity.ProfileImagePath;
        StreetAddress = entity.Address.StreetAddress;
        PostCode = entity.Address.PostCode;
        City = entity.Address.City;
        Day = DateConverterDay(entity.DateOfBirth);
        Month = DateConverterMonth(entity.DateOfBirth);
        Year = DateConverterYear(entity.DateOfBirth);
        
    }

    public static DateOnly DateConverter(int year, int month, int day)
    {
        DateOnly date = new DateOnly(year, month, day);
        return date;
    }

    public static int DateConverterDay(DateOnly dateOfBirth)
    {
        return dateOfBirth.Day;
    }
    public static int DateConverterMonth(DateOnly dateOfBirth)
    {
        return dateOfBirth.Month;
    }

    public static int DateConverterYear(DateOnly dateOfBirth)
    {
        return dateOfBirth.Year;
    }


}
