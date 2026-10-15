namespace RaceDayAPI.DTOs;

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber,
    string Role);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(
    int UserID,
    string FirstName,
    string LastName,
    string Email,
    string Role);