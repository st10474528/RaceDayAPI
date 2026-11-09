using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Middleware;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers;

[ApiController]
[Route("api/users")]
[AuthorizeRole("Organiser", "Participant")]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;

    /// <summary>Get the current user's profile.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound();

        return Ok(new UserProfileDto(user.UserID, user.FirstName, user.LastName,
            user.Email, user.PhoneNumber, user.ProfilePictureUrl, user.Role, user.CreatedAt));
    }

    /// <summary>Update the current user's profile.</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        await _db.SaveChangesAsync();
        return Ok(new UserProfileDto(user.UserID, user.FirstName, user.LastName,
            user.Email, user.PhoneNumber, user.ProfilePictureUrl, user.Role, user.CreatedAt));
    }

    /// <summary>Upload a profile picture (Part 3 requirement).</summary>
    [HttpPost("me/picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file, [FromServices] IImageService imageService)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound();
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var imageUrl = await imageService.UploadImageAsync(file);
        user.ProfilePictureUrl = imageUrl;
        await _db.SaveChangesAsync();

        return Ok(new { url = imageUrl });
    }
}