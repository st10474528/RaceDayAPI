using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models;

[Table("Result")]
public class Result
{
    [Key]
    public int ResultID { get; set; }

    public int EnrolmentID { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    public int FinishingPosition { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(EnrolmentID))]
    public Enrolment? Enrolment { get; set; }
}
