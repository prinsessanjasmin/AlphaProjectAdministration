using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class AddressRepository(DataContext context) : BaseRepository<AddressEntity>(context), IAddressRepository
{
    private readonly DataContext _context = context;
}
