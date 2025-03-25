using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class AppUserDto 
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!; 
}
