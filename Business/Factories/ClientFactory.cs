using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class ClientFactory
{
    public static ClientEntity Create (ClientFormModel form)
    {
        return new ClientEntity
        {
            ClientName = form.ClientName
        };  
    }

    public static ClientFormModel Create(ClientEntity entity)
    {
        return new ClientFormModel
        {
            ClientName = entity.ClientName,
        };
    }
}
