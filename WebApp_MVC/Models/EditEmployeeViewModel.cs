using Business.Models;
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class EditEmployeeViewModel
{
    public string Id { get; set; }

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

    public static implicit operator EmployeeDto(EditEmployeeViewModel model)
    {
        return model == null
            ? null!
            : new EmployeeDto
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
                DateOfBirth = DateConverter(model.Year, model.Month, model.Day),
            };
    }

    public EditEmployeeViewModel(ApplicationUser user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        JobTitle = user.JobTitle;
        ProfileImagePath = user.ProfileImagePath;
        StreetAddress = user.Address.StreetAddress;
        PostCode = user.Address.PostCode;
        City = user.Address.City;
        if (user.DateOfBirth.HasValue)
        {
            Year = DateConverterYear(user.DateOfBirth.Value);
            Month = DateConverterMonth(user.DateOfBirth.Value);
            Day = DateConverterDay(user.DateOfBirth.Value);
        }
        else
        {
            // Provide default values or leave them at their default
            Year = DateTime.Now.Year;
            Month = 1;
            Day = 1;
            // Or you could set them to 0 to indicate no date selected
        }

    }

    public static DateOnly DateConverter(int year, int month, int day)
    {

        // Get the last day of the selected month in the selected year
        int maxDay = DateTime.DaysInMonth(year, month);

        // If the selected day is greater than the maximum days in the month, adjust it
        if (day > maxDay)
        {
            day = maxDay;
        }

        try
        {
            
            DateOnly date = new(year, month, day);
            return date;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating DateOnly: {ex.Message}");
            throw; 
        }
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

    public EditEmployeeViewModel()
    {
        
    }
}
