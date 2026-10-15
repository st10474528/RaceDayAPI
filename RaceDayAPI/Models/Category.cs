using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models;

[Table("Category")]
public class Category
{
    [Key]
    public int CategoryID { get; set; }

    public int EventID { get; set; }

    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public int? MinimumAge { get; set; }
    public int? MaximumAge { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal? Distance { get; set; }

    [ForeignKey(nameof(EventID))]
    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}