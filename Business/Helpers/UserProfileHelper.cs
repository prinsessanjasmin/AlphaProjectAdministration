using Data.Entities;
using Microsoft.IdentityModel.Tokens;


namespace Business.Helpers;

public static class UserProfileHelper
{
    public static bool IsProfileComplete (ApplicationUser user)
    {
        return !string.IsNullOrWhiteSpace(user.FirstName)
            && !string.IsNullOrWhiteSpace(user.LastName)
            && !string.IsNullOrWhiteSpace(user.Email)
            && !string.IsNullOrWhiteSpace(user.JobTitle)
            && user.AddressId.HasValue
            && user.DateOfBirth != default;
    }
}

