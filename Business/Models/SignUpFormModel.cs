using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class SignUpFormModel 
{
    [Display(Name = "First Name", Prompt ="Enter your first name.")]
    [Required(ErrorMessage ="You must enter your first name.")]
    [PersonalData]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Display(Name = "Last Name", Prompt = "Enter your last name.")]
    [Required(ErrorMessage = "You must enter your last name.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email Address", Prompt = "Eg. nn@domain.xx ")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You must enter your email address.")]
    [RegularExpression("", ErrorMessage ="The email address is not in a valid format.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Minimum 8 characters, including at least one letter, one number and one special character.")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You must choose a password.")]
    [RegularExpression("^(?=.*[A-ZÅÄÖa-zåäö])(?=.*\\d)(?=.*[!@#$%^&*()_\\-+={}[\\]\\\\|:;'\\\"<>,.?/~`]).{8,}$", ErrorMessage = "Your password must be at least 8 characters long and contain both letters, numbers and a special character.")]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password", Prompt = "Repeat your password.")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "You must confirm your password.")]
    [Compare(nameof(Password), ErrorMessage ="The passwords do not match.")]
    public string ConfirmPassword{ get; set; } = null!;

    [Required (ErrorMessage = "You must confirm that you accept the terms and conditions.")]
    [Display(Name = "Terms and conditions", Prompt = "I accept the terms and coditions.")]
    public bool AcceptTerms { get; set; }


    public string Role { get; set; } = null!; 
}
