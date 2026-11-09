using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class ProfileController : Controller
{
    private readonly ApiClient _api;
    public ProfileController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null) return RedirectToAction("Login", "Auth");

        // 👇 Updated to match plan: /api/users/me
        var res = await _api.GetAsync("users/me");
        if (!res.IsSuccessStatusCode) return NotFound();

        var profile = await _api.ReadAsync<UserProfileDto>(res);
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string firstName, string lastName, string phoneNumber)
    {
        // 👇 Updated to match plan: PUT /api/users/me
        var res = await _api.PutAsync("users/me", new
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        });

        if (res.IsSuccessStatusCode)
        {
            var user = HttpContext.Session.GetUser();
            if (user != null)
            {
                HttpContext.Session.SetUser(user.Value.UserId, user.Value.Role, user.Value.Email, $"{firstName} {lastName}");
            }
            TempData["Success"] = "Profile updated successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to update profile.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UploadPicture(IFormFile picture)
    {
        if (picture != null && picture.Length > 0)
        {
          
            var res = await _api.PostFileAsync("users/me/picture", picture);
            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile picture uploaded!";
            }
            else
            {
                TempData["Error"] = "Failed to upload picture.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}