using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ProjectEmployeeRepository(DataContext context) : BaseRepository<ProjectEmployeeEntity>(context), IProjectEmployeeRepository
{
    private readonly DataContext _context = context;
}
