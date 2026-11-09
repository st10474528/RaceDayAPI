using System.Text.Json;

namespace RaceDayMVC.Services;

public static class SessionHelper
{
    private const string Key = "RaceDay.User";

    public static void SetUser(this ISession session, int userId, string role, string email, string name)
    {
        var payload = JsonSerializer.Serialize(new { UserId = userId, Role = role, Email = email, Name = name });
        session.SetString(Key, payload);
    }

    public static (int UserId, string Role, string Email, string Name)? GetUser(this ISession session)
    {
        var raw = session.GetString(Key);
        if (string.IsNullOrEmpty(raw)) return null;

        var doc = JsonDocument.Parse(raw).RootElement;
        return (
            doc.GetProperty("UserId").GetInt32(),
            doc.GetProperty("Role").GetString()!,
            doc.GetProperty("Email").GetString()!,
            doc.GetProperty("Name").GetString()!
        );
    }

    public static void ClearUser(this ISession session) => session.Remove(Key);
}