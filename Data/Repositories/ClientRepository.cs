using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public class ClientRepository(DataContext context) : BaseRepository<ClientEntity>(context), IClientRepository
{
    private readonly DataContext _context = context;

    public override async Task<IEnumerable<ClientEntity>> GetAsync()
    {

        try
        {
            var clientList = await _context.Set<ClientEntity>()
                .Include(c => c.Projects)
                .ThenInclude(p => p.Status)
                .ToListAsync();

            return clientList;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error finding clients :: {ex.Message}");
            return null!;
        }
    }


    public async override Task<ClientEntity> GetAsync(Expression<Func<ClientEntity, bool>> expression)
    {
        var client = await _context.Set<ClientEntity>()
            .Include(c => c.Projects)
            .ThenInclude(p => p.Status)
            .FirstOrDefaultAsync(expression);

        return client ?? new ClientEntity();
    }
}