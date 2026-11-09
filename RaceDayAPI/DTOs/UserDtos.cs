namespace RaceDayAPI.DTOs;

public record UserProfileDto(
    int UserID,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? ProfilePictureUrl, // 👈 Added here
    string Role,
    DateTime CreatedAt);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string? PhoneNumber);