using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class EmployeeRepository(DataContext context) : BaseRepository<EmployeeEntity>(context), IEmployeeRepository
{
    private readonly DataContext _context = context;

    public override async Task<IEnumerable<EmployeeEntity>> GetAsync()
    {
        return await _context.Set<EmployeeEntity>()
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project) 
            .ToListAsync();
    }

    public override async Task<EmployeeEntity> GetAsync(Expression<Func<EmployeeEntity, bool>> expression)
    {
        var employee = await _context.Set<EmployeeEntity>()
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(expression);

        return employee ?? new EmployeeEntity();
    }
}
