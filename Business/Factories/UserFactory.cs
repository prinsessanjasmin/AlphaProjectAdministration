using Business.Models;
using Data.Entities;
using Business.Helpers;

namespace Business.Factories;

public static class UserFactory
{
    public static ApplicationUser Create (SignUpFormModel model)
    {
        var appUser = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email
        };

        return appUser;
    }
}
