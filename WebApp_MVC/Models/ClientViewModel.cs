using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace WebApp_MVC.Models;

public class ClientViewModel(IClientService clientService)
{
    private readonly IClientService _clientService = clientService;
    public List<ClientEntity> Clients { get; set; } = [];

    public async Task GetClients()
    {
        var result = await _clientService.GetAllClients();
        

        if (result.Success)
        {
            var clientResult = result as Result<IEnumerable<ClientEntity>>;
            var clients = clientResult?.Data;
        
            if (clients != null)
            {
                Clients = clients.ToList();
            }
        }
    }
}
