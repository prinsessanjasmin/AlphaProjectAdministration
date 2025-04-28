using Business.Models;
using Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp_MVC.Models;

public class EditClientViewModel
{
    public int Id { get; set; } 

    [Display(Name = "Client Name", Prompt = "Enter the client's name.")]
    [Required(ErrorMessage = "You must enter the client's name.")]
    public string ClientName { get; set; } = null!;

    public static implicit operator ClientDto(EditClientViewModel model)
    {
        return model == null
            ? null!
            : new ClientDto
            {
                ClientName = model.ClientName
            };
    }

    public EditClientViewModel(ClientEntity client)
    {
        Id = client.Id;
        ClientName = client.ClientName;
    }

    public EditClientViewModel()
    {

    }
}
