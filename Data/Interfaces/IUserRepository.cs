using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Data.Interfaces;

public interface IUserRepository
{
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role);
    Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
}
