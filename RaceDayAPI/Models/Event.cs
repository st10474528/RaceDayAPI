using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models;

[Table("Event")]
public class Event
{
    [Key]
    public int EventID { get; set; }

    public int OrganiserID { get; set; }
    public int EventTypeID { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Column(TypeName = "decimal(6,2)")]
    public decimal Distance { get; set; }

    [MaxLength(500)]
    public string? BannerImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(OrganiserID))]
    public User? Organiser { get; set; }

    [ForeignKey(nameof(EventTypeID))]
    public EventType? EventType { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}