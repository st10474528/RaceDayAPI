using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using Xunit;

namespace RaceDayAPI.Tests;

public class EnrolmentsControllerTests
{
    private (EnrolmentsController controller, RaceDayAPI.Data.RaceDayDbContext db) Build(int userId, string role)
    {
        var db = TestDbFactory.Create();

        // Participant
        db.Users.Add(new User
        {
            UserID = 3,
            FirstName = "Tshilidzi",
            LastName = "Ndlovu",
            Email = "tshilidzi@example.com",
            PasswordHash = "x",
            Role = "Participant"
        });

        // Organiser for the event (FK)
        db.Users.Add(new User
        {
            UserID = 1,
            FirstName = "Thabo",
            LastName = "Mokoena",
            Email = "thabo@raceday.co.za",
            PasswordHash = "x",
            Role = "Organiser"
        });

        db.Events.Add(new Event
        {
            EventID = 1,
            OrganiserID = 1,
            EventTypeID = 1,
            Name = "City Run",
            EventDate = DateTime.UtcNow.AddDays(20),
            Location = "JHB",
            Distance = 10m
        });

        db.Categories.Add(new Category
        {
            CategoryID = 1,
            EventID = 1,
            CategoryName = "Senior"
        });

        db.SaveChanges();

        var http = new DefaultHttpContext();
        http.Items["UserId"] = userId;
        http.Items["Role"] = role;

        var controller = new EnrolmentsController(db)
        {
            ControllerContext = new ControllerContext { HttpContext = http }
        };
        return (controller, db);
    }

    [Fact]
    public async Task Enrol_AsParticipant_ReturnsCreated()
    {
        var (controller, _) = Build(3, "Participant");

        // 👇 FIXED: Passed eventId (1) as the first argument
        var result = await controller.Enrol(1, new CreateEnrolmentDto(1, 1));

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Enrol_Duplicate_ReturnsConflict()
    {
        var (controller, _) = Build(3, "Participant");

        // 👇 FIXED: Passed eventId (1)
        await controller.Enrol(1, new CreateEnrolmentDto(1, 1));
        var result = await controller.Enrol(1, new CreateEnrolmentDto(1, 1));

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Enrol_InvalidCategory_ReturnsBadRequest()
    {
        var (controller, _) = Build(3, "Participant");

        // 👇 FIXED: Passed eventId (1)
        var result = await controller.Enrol(1, new CreateEnrolmentDto(1, 999));

        Assert.IsType<BadRequestObjectResult>(result);
    }
}