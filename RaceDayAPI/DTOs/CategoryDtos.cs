namespace RaceDayAPI.DTOs;

public record CategoryDto(
    int CategoryID,
    int EventID,
    string CategoryName,
    int? MinimumAge,
    int? MaximumAge,
    decimal? Distance);

public record CreateCategoryDto(
    string CategoryName,
    int? MinimumAge,
    int? MaximumAge,
    decimal? Distance);
