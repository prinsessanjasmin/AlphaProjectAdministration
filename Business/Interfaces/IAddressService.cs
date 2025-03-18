using Business.Models;
using Data.Entities;

namespace Business.Interfaces; 

public interface IAddressService
{
    Task<IResult<AddressEntity>> CreateAddress(MemberFormModel form);
    Task<IResult> GetAllAddresses();
    Task<IResult> GetAddressByAddress(string streetAddress, string postCode, string city);
    Task<IResult> UpdateAddress();
    Task<IResult> DeleteAddress(int id); 
}
