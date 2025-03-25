using Business.Models;
using Data.Entities;

namespace Business.Factories;

public static class ClientFactory
{
    public static ClientEntity Create (ClientDto form)
    {
        return new ClientEntity
        {
            ClientName = form.ClientName
        };  
    }

    public static ClientDto Create(ClientEntity entity)
    {
        return new ClientDto
        {
            ClientName = entity.ClientName,
        };
    }
}
