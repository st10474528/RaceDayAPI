using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models;

[Table("EventType")]
public class EventType
{
    [Key]
    public int EventTypeID { get; set; }

    [Required, MaxLength(20)]
    public string TypeName { get; set; } = string.Empty;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}