using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests;

public class EventsControllerTests
{
    private (EventsController controller, RaceDayAPI.Data.RaceDayDbContext db) Build(int? userId, string? role)
    {
        var db = TestDbFactory.Create();
        db.Users.Add(new User
        {
            UserID = 1,
            FirstName = "Thabo",
            LastName = "Mokoena",
            Email = "thabo@raceday.co.za",
            PasswordHash = "x",
            Role = "Organiser"
        });
        db.EventTypes.Add(new EventType { EventTypeID = 1, TypeName = "Run" });
        db.SaveChanges();

        var http = new DefaultHttpContext();
        if (userId.HasValue) http.Items["UserId"] = userId.Value;
        if (role is not null) http.Items["Role"] = role;

        var controller = new EventsController(db)
        {
            ControllerContext = new ControllerContext { HttpContext = http }
        };
        return (controller, db);
    }

    [Fact]
    public async Task Create_AsOrganiser_ReturnsCreated()
    {
        var (controller, _) = Build(1, "Organiser");
        var result = await controller.Create(new CreateEventDto(
            1, "City Run", "Desc", DateTime.UtcNow.AddDays(30),
            "Johannesburg", 10m, null));
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var (controller, _) = Build(null, null);
        var result = await controller.GetAll();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_EventOwnedByAnotherOrganiser_ReturnsForbid()
    {
        var (controller, db) = Build(2, "Organiser");
        db.Events.Add(new Event
        {
            EventID = 1,
            OrganiserID = 1,
            EventTypeID = 1,
            Name = "X",
            EventDate = DateTime.UtcNow,
            Location = "Y",
            Distance = 5m
        });
        db.SaveChanges();

        var result = await controller.Update(1, new UpdateEventDto(
            1, "New", null, DateTime.UtcNow, "Loc", 5m, null));
        Assert.IsType<ForbidResult>(result);
    }
}