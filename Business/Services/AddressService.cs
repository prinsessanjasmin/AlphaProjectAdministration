using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class AddressService(IAddressRepository addressRepository) : IAddressService
{
    private readonly IAddressRepository _addressRepository = addressRepository;

    public async Task<IResult> CreateAddress(MemberFormModel form)
    {
        await _addressRepository.BeginTransactionAsync();
        try
        {
            if (form.StreetAddress == null || form.PostCode == null || form.City == null)
            {
                return Result.BadRequest("You need to fill out all address fields.");
            }

            string normailizedStreetAddress = form.StreetAddress.Trim().ToLower();
            string normalizedPostCode = form.PostCode.Trim();
            string normalizedCity = form.City.Trim().ToLower();
            
            var existingAddress = await _addressRepository.GetAsync(a =>
                a.StreetAddress == normailizedStreetAddress
                && a.PostCode == normalizedPostCode
                && a.City == normalizedCity);

            if (existingAddress != null) 
            {
                int id = existingAddress.Id;
                return Result<int>.Ok(id); 
            }
            else
            {
                AddressEntity addressEntity = AddressFactory.Create(normailizedStreetAddress, normalizedPostCode, normalizedCity);
                await _addressRepository.CreateAsync(addressEntity);
                await _addressRepository.SaveAsync();
                await _addressRepository.CommitTransactionAsync();
                return Result<AddressEntity>.Created(addressEntity); 
            }
        }
        catch
        {
            await _addressRepository.RollbackTransactionAsync();
            return Result.Error("Something went wrong.");
        }
    }

    public Task<IResult> DeleteAddress(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAddressByAddress(string streetAddress, string postCode, string city)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAllAddresses()
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateAddress()
    {
        throw new NotImplementedException();
    }
}
