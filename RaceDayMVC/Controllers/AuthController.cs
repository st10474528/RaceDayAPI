using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class AuthController : Controller
{
    private readonly ApiClient _api;
    public AuthController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string firstName, string lastName, string email,
        string password, string phoneNumber, string role)
    {
        var res = await _api.PostAsync("auth/register", new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            PhoneNumber = phoneNumber,
            Role = role
        });

        if (!res.IsSuccessStatusCode)
        {
            // 👇 THIS WILL SHOW THE EXACT ERROR FROM THE API 👇
            var errorDetails = await res.Content.ReadAsStringAsync();
            ViewBag.Error = $"API Error ({res.StatusCode}): {errorDetails}";
            return View();
        }

        TempData["Success"] = "Account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var res = await _api.PostAsync("auth/login", new { Email = email, Password = password });
        if (!res.IsSuccessStatusCode)
        {
            // 👇 THIS WILL SHOW THE EXACT ERROR FROM THE API 👇
            var errorDetails = await res.Content.ReadAsStringAsync();
            ViewBag.Error = $"API Error ({res.StatusCode}): {errorDetails}";
            return View();
        }

        _api.SaveCookieFromResponse(res);

        var user = await _api.ReadAsync<AuthUserDto>(res);
        if (user is null) { ViewBag.Error = "Login failed. Could not read user data."; return View(); }

        HttpContext.Session.SetUser(user.UserID, user.Role, user.Email, $"{user.FirstName} {user.LastName}");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.ClearUser();
        HttpContext.Session.Remove("ApiCookie");
        return RedirectToAction(nameof(Login));
    }
}

public record AuthUserDto(int UserID, string FirstName, string LastName, string Email, string Role);