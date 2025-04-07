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


    public int ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public ICollection<ProjectEmployeeEntity> TeamMembers { get; set; } = [];
    //Got help from ChatGPT 4o on this one ^

    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;

    public string? ProjectImagePath { get; set; }

    //The following members are (somewhat edited by myself) suggestions from Claude AI to provide a value for the Time left indicator in my Project view. 
    [NotMapped]
    public int DaysRemaining => (EndDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days;

    [NotMapped]
    public string TimeStatus => DaysRemaining < 0 ? "Overdue" :
                               DaysRemaining < 7 ? "Due Soon" : "On Track";

    [NotMapped]
    public string FormattedTimeRemaining
    {
        get
        {
            if (DaysRemaining < 0)
            {
                return $"{Math.Abs(DaysRemaining)} days ago";
            }
            else if (DaysRemaining == 0)
            {
                return "Due today";
            }
            else if (DaysRemaining == 1)
            {
                return "1 day left";
            }
            else if (DaysRemaining > 1 && DaysRemaining < 7)
            {
                return $"{DaysRemaining} days left";
            }
            else if (DaysRemaining > 7 && DaysRemaining < 11)
            {
                return "1 week left";
            }
            else
            {
                int weeks = DaysRemaining / 7;
                int remainingDays = DaysRemaining % 7;

                if (remainingDays > 4)
                {
                    return $"{weeks + 1} weeks left";
                }
                else
                {
                    return $"{weeks + 1} weeks left";
                }
            }
        }
    }
}
