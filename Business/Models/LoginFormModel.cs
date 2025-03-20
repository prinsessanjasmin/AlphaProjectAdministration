using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class LoginFormModel
{
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You must enter your email address.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter your password")]
    [Required(ErrorMessage = "You must enter your password.")]
    [DataType (DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
