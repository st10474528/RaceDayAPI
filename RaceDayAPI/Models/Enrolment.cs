using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models;

[Table("Enrolment")]
public class Enrolment
{
    [Key]
    public int EnrolmentID { get; set; }

    public int ParticipantID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [ForeignKey(nameof(ParticipantID))]
    public User? Participant { get; set; }

    [ForeignKey(nameof(EventID))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(CategoryID))]
    public Category? Category { get; set; }

    public Result? Result { get; set; }
}