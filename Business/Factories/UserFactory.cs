using Business.Models;
using Data.Entities;
using Business.Helpers;

namespace Business.Factories;

public static class UserFactory
{
    public static UserEntity Create (SignUpFormModel model)
    {
        var userEntity = new UserEntity
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
        };

       (userEntity.EncryptedPassword, userEntity.SecurityKey) = PasswordGenerator.Generate(model.Password);

       return userEntity;
    }
}
