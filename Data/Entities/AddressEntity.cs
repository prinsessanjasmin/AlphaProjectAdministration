using System.ComponentModel.DataAnnotations;

namespace Data.Entities; 

public class AddressEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string StreetAddress { get; set; } = null!;

    [Required]
    public string PostCode { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();

}
