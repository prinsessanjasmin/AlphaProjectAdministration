using Business.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models
{
    public class ClientFormViewModel
    {
        [Display(Name = "Client Name", Prompt = "Enter the client's name.")]
        [Required(ErrorMessage = "You must enter the client's name.")]
        public string ClientName { get; set; } = null!;

        public static implicit operator ClientDto(ClientFormViewModel model)
        {
            return model == null
                ? null!
                : new ClientDto
                {
                    ClientName = model.ClientName
                };
        }
    }
}
