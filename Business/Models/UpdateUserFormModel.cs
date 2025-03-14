using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class UpdateUserFormModel(int id)
{
    public int Id { get; private set; } = id; 

    [Display(Name = "First Name", Prompt = "Enter your first name.")]
    [Required(ErrorMessage = "You must enter your first name.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter your last name.")]
    [Required(ErrorMessage = "You must enter your last name.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email Address", Prompt = "Eg. nn@domain.xx ")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "You must enter your email address.")]
    [RegularExpression("", ErrorMessage = "The email address is not in a valid format.")]
    public string Email { get; set; } = null!;
}
