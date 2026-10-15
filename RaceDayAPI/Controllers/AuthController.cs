using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ISessionService _session;

    public AuthController(RaceDayDbContext db, IPasswordHasher hasher, ISessionService session)
    {
        _db = db; _hasher = hasher; _session = session;
    }

    /// <summary>Register a new user (Organiser or Participant).</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        if (dto.Role != "Organiser" && dto.Role != "Participant")
            return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email already registered." });

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            PasswordHash = _hasher.Hash(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new AuthResponseDto(
            user.UserID, user.FirstName, user.LastName, user.Email, user.Role));
    }

    /// <summary>Log in and create a session for the user.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials." });

        _session.SetUser(HttpContext, user.UserID, user.Role, user.Email);
        return Ok(new AuthResponseDto(user.UserID, user.FirstName,
            user.LastName, user.Email, user.Role));
    }

    /// <summary>Log out the current session.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _session.Clear(HttpContext);
        return NoContent();
    }

    /// <summary>Get the currently authenticated user.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var info = _session.GetUser(HttpContext);
        if (info is null) return Unauthorized();

        var user = await _db.Users.FindAsync(info.Value.UserId);
        if (user is null) return Unauthorized();

        return Ok(new AuthResponseDto(user.UserID, user.FirstName,
            user.LastName, user.Email, user.Role));
    }
}