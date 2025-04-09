using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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

    public async Task<IEnumerable<EmployeeEntity>> SearchByTermAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return Enumerable.Empty<EmployeeEntity>();
        }

        searchTerm = searchTerm.Trim().ToLower();

        try
        {
            return await _context.Employees
                .Where(e => e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    (e.FirstName + " " + e.LastName).ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error searching employees: {ex.Message}");
            return Enumerable.Empty<EmployeeEntity>();
        }
    }
}
