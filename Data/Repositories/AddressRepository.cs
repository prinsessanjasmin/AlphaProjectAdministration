using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class AddressRepository(DataContext context) : BaseRepository<AddressEntity>(context), IAddressRepository
{
    private readonly DataContext _context = context;

    public override Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public override Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public override Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}
