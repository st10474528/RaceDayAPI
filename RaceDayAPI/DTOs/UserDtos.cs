namespace RaceDayAPI.DTOs;

public record UserProfileDto(
    int UserID,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Role,
    DateTime CreatedAt);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string? PhoneNumber);