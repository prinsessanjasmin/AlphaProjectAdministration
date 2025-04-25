using Business.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class UpdateUserViewModel
{
    [Display(Name = "Phone number", Prompt = "Enter phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Job Title", Prompt = "Enter job title")]
    [Required(ErrorMessage = "Required")]
    public string JobTitle { get; set; } = null!;

    [Display(Name = "Profile Image", Prompt = "Upload a profile image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProfileImage { get; set; }

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

    public static implicit operator UpdateUserDto(UpdateUserViewModel model)
    {
        return model == null
            ? null!
            : new UpdateUserDto
            {
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                DateOfBirth = DateConverter(model.Day, model.Month, model.Year),
                StreetAddress = model.StreetAddress,
                PostCode = model.PostCode,
                City = model.City,
            };
    }

    public static DateOnly DateConverter(int year, int month, int day)
    {
        DateOnly date = new(day, month, year);
        return date;
    }
}
