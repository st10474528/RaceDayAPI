using Microsoft.AspNetCore.Http;
using RaceDayAPI.Services;

namespace RaceDayAPI.Tests;

public class FakeSessionService : ISessionService
{
    private (int, string, string)? _current;

    public void SetUser(HttpContext context, int userId, string role, string email)
        => _current = (userId, role, email);

    public (int? UserId, string? Role)? GetUser(HttpContext context)
        => _current is null ? null : (_current.Value.Item1, _current.Value.Item2);

    public void Clear(HttpContext context) => _current = null;
}