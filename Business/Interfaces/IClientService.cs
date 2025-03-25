using Data.Entities;
using Business.Models;

namespace Business.Interfaces;

public interface IClientService
{
    Task<IResult> CreateClient(ClientDto form);
    Task<IResult> GetAllClients();
    Task<IResult> GetClientById(int id);
    Task<IResult> UpdateClient(int id, ClientEntity updatedClient);
    Task<IResult> DeleteClient(int id);
}
