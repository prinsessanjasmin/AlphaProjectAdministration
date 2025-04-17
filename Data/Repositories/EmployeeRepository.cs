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
            .ThenInclude(p => p.Status)
            .ToListAsync();
    }

    public override async Task<EmployeeEntity> GetAsync(Expression<Func<EmployeeEntity, bool>> expression)
    {
        var employee = await _context.Set<EmployeeEntity>()
            .Include (e => e.Address)
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .ThenInclude(p => p.Status)
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

    public override async Task<EmployeeEntity> UpdateAsync(Expression<Func<EmployeeEntity, bool>> expression, EmployeeEntity updatedEntity)
    {
        try
        {
            var existingEntity = await _context.Employees
                .Include(e => e.Address) 
                .FirstOrDefaultAsync(expression);

            if (existingEntity == null)
                return null!;

            var updatedValues = _context.Entry(updatedEntity).CurrentValues;
            var existingEntry = _context.Entry(existingEntity);

            var keyName = existingEntry.Metadata?.FindPrimaryKey()?.Properties
                .Select(p => p.Name).SingleOrDefault();

            foreach (var property in updatedValues.Properties)
            {
                if (property.Name != keyName)
                {
                    var value = updatedValues[property.Name];
                    existingEntry.Property(property.Name).CurrentValue = value;
                }
            }

            // Update navigation properties like Address if needed
            if (updatedEntity.Address != null)
            {
                if (existingEntity.Address == null)
                {
                    existingEntity.Address = new AddressEntity();
                }
                _context.Entry(existingEntity.Address).CurrentValues.SetValues(updatedEntity.Address);
            }

            return existingEntity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating employee :: {ex.Message}");
            return null!;
        }
    }

}
