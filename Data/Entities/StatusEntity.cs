using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class StatusEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(20)")]
    public string StatusName { get; set; } = null!;

    public ICollection<ProjectEntity> Projects { get; set; } = [];
}
