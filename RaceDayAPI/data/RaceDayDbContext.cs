using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RaceDayAPI.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<EventType>().ToTable("EventType");
        modelBuilder.Entity<Event>().ToTable("Event");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Enrolment>().ToTable("Enrolment");
        modelBuilder.Entity<Result>().ToTable("Result");

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<EventType>().HasIndex(e => e.TypeName).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => new { c.EventID, c.CategoryName }).IsUnique();
        modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantID, e.EventID }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(r => r.EnrolmentID).IsUnique();

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint("CK_User_Role",
                "[Role] IN ('Organiser', 'Participant')"));

        modelBuilder.Entity<Event>()
            .ToTable(t => t.HasCheckConstraint("CK_Event_Distance", "[Distance] > 0"));

        modelBuilder.Entity<Category>()
            .ToTable(t => t.HasCheckConstraint("CK_Category_MinimumAge",
                "[MinimumAge] IS NULL OR [MinimumAge] >= 0"));

        modelBuilder.Entity<Category>()
            .ToTable(t => t.HasCheckConstraint("CK_Category_MaximumAge",
                "[MaximumAge] IS NULL OR [MaximumAge] >= 0"));

        modelBuilder.Entity<Category>()
            .ToTable(t => t.HasCheckConstraint("CK_Category_AgeRange",
                "[MinimumAge] IS NULL OR [MaximumAge] IS NULL OR [MinimumAge] <= [MaximumAge]"));

        modelBuilder.Entity<Category>()
            .ToTable(t => t.HasCheckConstraint("CK_Category_Distance",
                "[Distance] IS NULL OR [Distance] > 0"));

        modelBuilder.Entity<Enrolment>()
            .ToTable(t => t.HasCheckConstraint("CK_Enrolment_Status",
                "[Status] IN ('Pending', 'Confirmed', 'Cancelled')"));

        modelBuilder.Entity<Result>()
            .ToTable(t => t.HasCheckConstraint("CK_Result_Position", "[FinishingPosition] > 0"));

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.EventID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(e => e.Result)
            .HasForeignKey<Result>(r => r.EnrolmentID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventType>().HasData(
            new EventType { EventTypeID = 1, TypeName = "Run" },
            new EventType { EventTypeID = 2, TypeName = "Walk" },
            new EventType { EventTypeID = 3, TypeName = "Cycle" }
        );
    }
}
