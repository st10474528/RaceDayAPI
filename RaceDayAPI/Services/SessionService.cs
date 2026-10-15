using System.Text.Json;

namespace RaceDayAPI.Services;

public class SessionService : ISessionService
{
    private const string Key = "RaceDay.User";

    public void SetUser(HttpContext context, int userId, string role, string email)
    {
        var payload = JsonSerializer.Serialize(new { UserId = userId, Role = role, Email = email });
        context.Session.SetString(Key, payload);
    }

    public (int? UserId, string? Role)? GetUser(HttpContext context)
    {
        var raw = context.Session.GetString(Key);
        if (string.IsNullOrEmpty(raw)) return null;

        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;
        return (root.GetProperty("UserId").GetInt32(), root.GetProperty("Role").GetString());
    }

    public void Clear(HttpContext context) => context.Session.Remove(Key);
}
