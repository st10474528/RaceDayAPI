namespace RaceDayMVC.Models;

public class EventDto
{
    public int EventID { get; set; }
    public int OrganiserID { get; set; }
    public int EventTypeID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public string? BannerImageUrl { get; set; }
}

public class CategoryDto
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? MinimumAge { get; set; }
    public int? MaximumAge { get; set; }
    public decimal? Distance { get; set; }
}

public class EnrolmentDto
{
    public int EnrolmentID { get; set; }
    public int ParticipantID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ResultDto
{
    public int ResultID { get; set; }
    public int EnrolmentID { get; set; }
    public string FinishTime { get; set; } = string.Empty;
    public int FinishingPosition { get; set; }
    public DateTime RecordedAt { get; set; }
}