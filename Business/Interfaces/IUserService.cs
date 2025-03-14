using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IUserService
{
    Task<IResult> CreateUser(SignUpFormModel form);
    Task<IResult> GetAllUsers();
    Task<IResult> GetUserById(int id);
    Task<IResult> GetUserByEmail(string email);
    Task<IResult> UpdateUser(int id, UserEntity updatedUser); //OBS: använd den andra modellen? 
    Task<IResult> DeleteProject(int id);
}
