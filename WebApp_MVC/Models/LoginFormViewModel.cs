using Business.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class LoginFormViewModel
{
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Required")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter your password")]
    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; } 
}
