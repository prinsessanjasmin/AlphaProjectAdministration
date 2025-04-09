using Business.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class SignUpViewModel
{
    [Display(Name = "First Name", Prompt = "Your first name")]
    [Required(ErrorMessage = "Required")]
    [PersonalData]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Display(Name = "Last Name", Prompt = "Your last name")]
    [Required(ErrorMessage = "Required")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email Address", Prompt = "Your email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter a safe password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Required")]
    [RegularExpression("^(?=.*[A-ZÅÄÖa-zåäö])(?=.*\\d)(?=.*[!@#$%^&*()_\\-+={}[\\]\\\\|:;'\\\"<>,.?/~`]).{8,}$", ErrorMessage = "Invalid password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password", Prompt = "Repeat your password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Required")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Terms and conditions", Prompt = "I accept the terms and coditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Required")]
    public bool AcceptTerms { get; set; }

    [Required(ErrorMessage = "Required")]
    public string Role { get; set; } = null!;

    public static implicit operator AppUserDto(SignUpViewModel model)
    {
        return model == null
            ? null!
            : new AppUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
            };
    }
}
