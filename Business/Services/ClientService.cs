using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<IResult> CreateClient(ClientFormModel form)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> DeleteClient(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetAllClients()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetClientById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> UpdateClient(int id, ClientEntity updatedClient)
    {
        throw new NotImplementedException();
    }
}
