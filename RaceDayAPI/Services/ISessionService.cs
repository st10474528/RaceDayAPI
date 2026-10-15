namespace RaceDayAPI.Services;

public interface ISessionService
{
    void SetUser(HttpContext context, int userId, string role, string email);
    (int? UserId, string? Role)? GetUser(HttpContext context);
    void Clear(HttpContext context);
}