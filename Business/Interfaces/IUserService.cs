using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IUserService
{
    Task<IResult> CreateUser(AppUserDto form);
    Task<IResult> GetAllUsers();
    Task<IResult> GetUserById(string id);
    Task<IResult> GetUserByEmail(string email);
    Task<IResult> UpdateUser(string id, ApplicationUser updatedUser); //OBS: använd den andra modellen? 
    Task<IResult> DeleteUser(string id);
}
