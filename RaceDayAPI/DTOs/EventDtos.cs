namespace RaceDayAPI.DTOs;

public record EventDto(
    int EventID,
    int OrganiserID,
    int EventTypeID,
    string Name,
    string? Description,
    DateTime EventDate,
    string Location,
    decimal Distance,
    string? BannerImageUrl,
    DateTime CreatedAt);

public record CreateEventDto(
    int EventTypeID,
    string Name,
    string? Description,
    DateTime EventDate,
    string Location,
    decimal Distance,
    string? BannerImageUrl);

public record UpdateEventDto(
    int EventTypeID,
    string Name,
    string? Description,
    DateTime EventDate,
    string Location,
    decimal Distance,
    string? BannerImageUrl);