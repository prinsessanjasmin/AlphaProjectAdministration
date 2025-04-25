using Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IUserRepository
{
    #region Transaction Management
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    #endregion

    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role);
    Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<IEnumerable<ApplicationUser>> SearchByTermAsync(string searchTerm);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<bool> AlreadyExistsAsync(Expression<Func<ApplicationUser, bool>> expression);
    Task<int> SaveAsync();
}
