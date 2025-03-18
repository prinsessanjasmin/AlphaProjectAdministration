using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<IResult> CreateClient(ClientFormModel form)
    {
        await _clientRepository.BeginTransactionAsync();
        try
        {
            if (form == null)
            {
                return Result.BadRequest("You need to fill out the form.");
            }

            if (await _clientRepository.AlreadyExistsAsync(x => x.ClientName == form.ClientName))
            {
                return Result.AlreadyExists("A client with the same name already exists.");
            }

            var entity = ClientFactory.Create(form);
            ClientEntity client = await _clientRepository.CreateAsync(entity);
            await _clientRepository.SaveAsync();
            await _clientRepository.CommitTransactionAsync();
            return Result<ClientEntity>.Created(client);
        }
        catch
        {
            await _clientRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> GetAllClients()
    {
        IEnumerable<ClientEntity> clients = new List<ClientEntity>();

        try
        {
            clients = await _clientRepository.GetAsync();
            if (clients == null)
            {
                return Result.NotFound("There are no clients registered.");
            }

            return Result<IEnumerable<ClientEntity>>.Ok(clients);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> GetClientById(int id)
    {
        try
        {
            ClientEntity client = await _clientRepository.GetAsync(e => e.Id == id);
            if (client == null)
            {
                return Result.NotFound("An client with that id wasn't found.");
            }

            return Result<ClientEntity>.Ok(client);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> UpdateClient(int id, ClientEntity updatedClient)
    {
        await _clientRepository.BeginTransactionAsync();
        try
        {
            ClientEntity client = await _clientRepository.UpdateAsync(e => e.Id == id, updatedClient);
            if (client == null)
                return Result.NotFound("Could not find the client in the database.");

            await _clientRepository.SaveAsync();
            await _clientRepository.CommitTransactionAsync();
            return Result<ClientEntity>.Ok(client);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteClient(int id)
    {
        await _clientRepository.BeginTransactionAsync();
        try
        {
            bool deleted = await _clientRepository.DeleteAsync(e => e.Id == id);
            if (deleted)
            {
                await _clientRepository.SaveAsync();
                await _clientRepository.CommitTransactionAsync();
                return Result.Ok();
            }

            return Result.Error("Something went wrong.");

        }
        catch
        {
            await _clientRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }

}
