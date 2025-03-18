using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;

namespace Business.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IResult> CreateUser(SignUpFormModel form)
    {
        await _userRepository.BeginTransactionAsync();

        if (!form.AcceptTerms)
        {
            return Result.BadRequest("You have to accept the terms and conditions to proceed."); 
        }

        if (await _userRepository.AlreadyExistsAsync(x => x.Email == form.Email))
        {
            return Result.AlreadyExists("The email adress you are trying to register already exists."); 
        }

        try
        {
            UserEntity userEntity = UserFactory.Create(form);
            await _userRepository.CreateAsync(userEntity);
            await _userRepository.SaveAsync();
            await _userRepository.CommitTransactionAsync();
            
            return Result<UserEntity>.Created(userEntity);
        }
        catch
        {
            await _userRepository.RollbackTransactionAsync();
            return Result.Error("Unable to create user.");
        }
    }

    public async Task<IResult> GetAllUsers()
    {
        try
        {
            IEnumerable<UserEntity> users = await _userRepository.GetAsync();
            if (users == null)
            {
                return Result.NotFound("No users were found.");
            }
                
            return Result<IEnumerable<UserEntity>>.Ok(); 
        }
        catch
        {
            return Result.Error("Something went wrong."); 
        }
    }

    public async Task<IResult> GetUserByEmail(string email)
    {
       try
        {
            UserEntity user = await _userRepository.GetAsync(x => x.Email == email);
            if (user == null)
            {
                return Result.NotFound("The user couldn't be found. Are you sure you entered the correct email address?");
            }
            return Result<UserEntity>.Ok(user); 
        }
        catch
        {
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> GetUserById(int id)
    {
        try
        {
            UserEntity user = await _userRepository.GetAsync(x => x.Id == id);
            if (user == null)
            {
                return Result.NotFound("The user couldn't be found.");
            }
            return Result<UserEntity>.Ok(user);
        }
        catch
        {
            return Result.Error("Something went wrong.");
        }
    }

    public async Task<IResult> UpdateUser(int id, UserEntity updatedUser) //Eller använd den andra modellen? 
    {
        await _userRepository.BeginTransactionAsync();
        try
        {
            UserEntity user = await _userRepository.UpdateAsync(u => u.Id == id, updatedUser);
            if (user == null)
                return Result.NotFound("Could not find the user in the database.");

            await _userRepository.SaveAsync();
            await _userRepository.CommitTransactionAsync();
            return Result<UserEntity>.Ok(user);
        }
        catch
        {
            return Result.Error("Something went wrong");
        }
    }

    public async Task<IResult> DeleteProject(int id)
    {
        try
        {
            await _userRepository.BeginTransactionAsync();
            UserEntity user = await _userRepository.GetAsync(x => x.Id == id);
            if (user == null)
            {
                return Result.NotFound("The user you want to delete couldn't be found.");
            }
            await _userRepository.DeleteAsync(x => x.Id == id);
            await _userRepository.CommitTransactionAsync();
            
            return Result.Ok();
        }
        catch
        {
            await _userRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }
}
