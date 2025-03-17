using Data.Entities;

namespace Business.Factories;

public static class AddressFactory
{
    public static AddressEntity Create (string streetAddress, string postCode, string city)
    {
        return new AddressEntity
        {
            StreetAddress = streetAddress,
            PostCode = postCode,
            City = city
        };
    }

    public static List<string> Create(AddressEntity address)
    {
        return new List<string> { address.StreetAddress, address.PostCode, address.City};    
    }
}
