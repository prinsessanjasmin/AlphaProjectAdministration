using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly DataContext _context = context;

    private IDbContextTransaction _transaction = null!;

    #region Transaction Management
    public virtual async Task BeginTransactionAsync()
    {
        _transaction ??= await _context.Database.BeginTransactionAsync();
    }

    public virtual async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }

    public virtual async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }
    #endregion

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role)
    {
        if (await _userManager.Users.AnyAsync(u => u.Email == user.Email))
        {
            return IdentityResult.Failed(); 
        }

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
        }
        return result;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        var userList = await _context.Users
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .ThenInclude(p => p.Status)
            .ToListAsync(); 
        return userList;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        var userWithRelatedData = await _context.Users
            .Include(u => u.Address)
            .Include(u => u.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
                .ThenInclude(p => p.Status)
            .FirstOrDefaultAsync(u => u.Id == id);

        return userWithRelatedData ?? user;
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return null;

        var userWithRelatedData = await _context.Users
            .Include(u => u.Address)
            .Include(u => u.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
                .ThenInclude(p => p.Status)
            .FirstOrDefaultAsync(u => u.Email == email);

        return userWithRelatedData ?? user;
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser updatedUser)
    {

        try
        {
            var existingUser = await _context.Users
                .Include(u => u.Address)
               .FirstOrDefaultAsync(u => u.Id == updatedUser.Id);

            if (existingUser == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var updatedValues = _context.Entry(updatedUser).CurrentValues;
            var existingEntry = _context.Entry(existingUser);

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
            if (updatedUser.Address != null)
            {
                existingUser.Address ??= new AddressEntity();
                _context.Entry(existingUser.Address).CurrentValues.SetValues(updatedUser.Address);
            }

            await _context.SaveChangesAsync();
            return await _userManager.UpdateAsync(existingUser);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating user :: {ex.Message}");
            return IdentityResult.Failed(new IdentityError { Description = $"Error updating user: {ex.Message}" });
        }
    }
    

    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }


    public async Task<IEnumerable<ApplicationUser>> SearchByTermAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return [];
        }

        searchTerm = searchTerm.Trim().ToLower();

        try
        {
            return await _context.Users
                .Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    (u.FirstName + " " + u.LastName).ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error searching users: {ex.Message}");
            return [];
        }
    }

    public async Task<bool> AlreadyExistsAsync(Expression<Func<ApplicationUser, bool>> expression)
    {
        try
        {
            return await _context.Users.AnyAsync(expression);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error :: {ex.Message}");
            return false;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
