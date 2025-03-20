using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public int ProjectId { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string ProjectName { get; set; } = null!;

    [Column(TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    [Required]
    [DataType(DataType.Date)]   
    public DateOnly StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]  
    public DateOnly EndDate { get; set; }

    public decimal? Budget { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public int ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public ICollection<ProjectEmployeeEntity> TeamMembers { get; set; } = [];
    //Got help from ChatGPT 4o on this one ^

    [Required]
    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;

    [NotMapped]
    public IFormFile? ProjectImage { get; set; }

}
