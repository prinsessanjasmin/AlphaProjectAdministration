using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class ClientRepository(DataContext context) : BaseRepository<ClientEntity>(context), IClientRepository
{
    private readonly DataContext _context = context;

    public async override Task<ClientEntity> GetAsync(Expression<Func<ClientEntity, bool>> expression)
    {
        var client = await _context.Set<ClientEntity>()
            .Include(c => c.Projects)
            .ThenInclude(p => p.Status)
            .FirstOrDefaultAsync(expression);

        return client ?? new ClientEntity();
    }
}
