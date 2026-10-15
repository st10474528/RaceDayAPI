using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.DTOs;
using RaceDayAPI.Services;
using Xunit;

namespace RaceDayAPI.Tests;

public class AuthControllerTests
{
    private (AuthController controller, FakeSessionService session) Build()
    {
        var db = TestDbFactory.Create();
        var session = new FakeSessionService();
        var hasher = new PasswordHasher();
        var controller = new AuthController(db, hasher, session)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        return (controller, session);
    }

    [Fact]
    public async Task Register_ValidOrganiser_ReturnsCreated()
    {
        var (controller, _) = Build();
        var result = await controller.Register(new RegisterDto(
            "Thabo", "Mokoena", "thabo@raceday.co.za", "Pass@123", "0821111111", "Organiser"));
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        var (controller, _) = Build();
        var result = await controller.Register(new RegisterDto(
            "X", "Y", "x@y.com", "Pass@123", null, "Admin"));
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var (controller, _) = Build();
        await controller.Register(new RegisterDto(
            "A", "B", "dup@x.com", "Pass@123", null, "Participant"));
        var result = await controller.Register(new RegisterDto(
            "C", "D", "dup@x.com", "Pass@123", null, "Participant"));
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Login_CorrectCredentials_CreatesSession()
    {
        var (controller, session) = Build();
        await controller.Register(new RegisterDto(
            "Sipho", "Dlamini", "sipho@example.com", "Pass@123", null, "Participant"));
        var result = await controller.Login(new LoginDto("sipho@example.com", "Pass@123"));
        Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(session.GetUser(new DefaultHttpContext()));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var (controller, _) = Build();
        await controller.Register(new RegisterDto(
            "Sipho", "Dlamini", "sipho2@example.com", "Pass@123", null, "Participant"));
        var result = await controller.Login(new LoginDto("sipho2@example.com", "Wrong"));
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}