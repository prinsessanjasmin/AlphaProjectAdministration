using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ClientFormModel
{
    [Display(Name = "Client Name", Prompt = "Enter the client's name.")]
    [Required(ErrorMessage = "You must enter the client's name.")]
    public string ClientName { get; set; } = null!;
}
