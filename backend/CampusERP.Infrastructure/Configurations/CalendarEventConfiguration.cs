using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.EventType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Color)
            .HasMaxLength(20);

        builder.Property(x => x.RecurrenceRule)
            .HasMaxLength(500);

        builder.Property(x => x.IsFullDay)
            .HasDefaultValue(false);

        builder.Property(x => x.IsRecurring)
            .HasDefaultValue(false);

        builder.Property(x => x.AffectsTimetable)
            .HasDefaultValue(true);

        builder.Property(x => x.Priority)
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicSession)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Semester)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Section)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.CalendarEvents)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.StartDate,
            x.EndDate
        });

        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.StartDate,
            x.EndDate
        });

        builder.HasIndex(x => new
        {
            x.DepartmentId,
            x.StartDate
        });

        builder.HasIndex(x => new
        {
            x.CourseId,
            x.StartDate
        });

        builder.HasIndex(x => new
        {
            x.SemesterId,
            x.StartDate
        });

        builder.HasIndex(x => new
        {
            x.SectionId,
            x.StartDate,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.StartDate,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.RoomId,
            x.StartDate,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.EventType,
            x.StartDate
        });

        builder.HasIndex(x => new
        {
            x.CampusId,
            x.StartDate,
            x.EventType,
            x.IsActive
        });

        builder.HasIndex(x => x.IsActive);
    }
}