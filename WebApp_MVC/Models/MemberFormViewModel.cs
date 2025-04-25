using Business.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class MemberFormViewModel
{
    [Display(Name = "First Name", Prompt = "Enter team member's first name.")]
    [Required(ErrorMessage = "You must enter the team member's first name.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "First Name", Prompt = "Enter team member's last name.")]
    [Required(ErrorMessage = "You must enter the team member's last name.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email Address", Prompt = "Enter team member's email address.")]
    [Required(ErrorMessage = "You must enter the team member's email address.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number", Prompt = "Enter team member's phone number (optional).")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Job Title", Prompt = "Enter team member's job title.")]
    [Required(ErrorMessage = "You must enter the team member's job title.")]
    public string JobTitle { get; set; } = null!;

    [Display(Name = "Street Address", Prompt = "Enter team member's street address.")]
    [Required(ErrorMessage = "You must enter the team member's street address.")]
    public string StreetAddress { get; set; } = null!;

    [Display(Name = "Post Code", Prompt = "Enter team member's post code.")]
    [Required(ErrorMessage = "You must enter the team member's post code.")]
    public string PostCode { get; set; } = null!;

    [Display(Name = "City", Prompt = "Enter team member's city of residence.")]
    [Required(ErrorMessage = "You must enter the team member's city of residence.")]
    public string City { get; set; } = null!;

    [Display(Name = "Date of Birth", Prompt = "Enter team member's date of birth.")]
    [Required(ErrorMessage = "You must enter the team member's date of birth.")]
    public DateOnly DateOfBirth { get; set; }

    [Display(Name = "Profile Picture", Prompt = "Upload a profile picture")]
    public IFormFile? ProfileImage { get; set; }

    public static implicit operator EmployeeDto(MemberFormViewModel model)
    {
        return model == null
            ? null!
            : new EmployeeDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                StreetAddress = model.StreetAddress,
                PostCode = model.PostCode,
                City = model.City,
                DateOfBirth = model.DateOfBirth
            };
    }
}
