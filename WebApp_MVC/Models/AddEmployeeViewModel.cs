using Business.Models;
using Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApp_MVC.Models;

public class AddEmployeeViewModel
{
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

    [Display(Name = "Date of Birth")]
    [Required(ErrorMessage = "Required")]
    public int Day { get; set; }

    [Required(ErrorMessage = "Required")]
    public int Month { get; set; }

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

    public static implicit operator MemberDto(AddEmployeeViewModel model)
    {
        return model == null
            ? null!
            : new MemberDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                StreetAddress = model.StreetAddress,
                PostCode = model.PostCode,
                City = model.City,
                //DateOfBirth = model.Day, model.Month, model.Year
            };
    }
}
