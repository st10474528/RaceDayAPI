using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Tests;

public static class TestDbFactory
{
    public static RaceDayDbContext Create()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique per call
            .Options;

        var db = new RaceDayDbContext(options);

       
        return db;
    }
}