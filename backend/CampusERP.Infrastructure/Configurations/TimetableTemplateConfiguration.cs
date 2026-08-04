using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class TimetableTemplateConfiguration: IEntityTypeConfiguration<TimetableTemplate>
{
    public void Configure(EntityTypeBuilder<TimetableTemplate> builder)
    {
        builder.ToTable("TimetableTemplates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .HasConversion<int>();

        builder.Property(x => x.LectureType)
            .HasConversion<int>();

        builder.Property(x => x.Priority)
            .HasDefaultValue(100);

        builder.Property(x => x.GenerateAttendance)
            .HasDefaultValue(true);

        builder.Property(x => x.IsOnline)
            .HasDefaultValue(false);

        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(1);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.MeetingLink)
            .HasMaxLength(500);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicSession)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TeacherAssignment)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.TeacherAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Section)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SemesterSubject)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.SemesterSubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.TimetableTemplates)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.LectureOverrides)
            .WithOne(x => x.TimetableTemplate)
            .HasForeignKey(x => x.TimetableTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TeacherAssignmentId,
            x.DayOfWeek,
            x.StartTime,
            x.ValidFrom,
            x.ValidTo
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.DayOfWeek,
            x.StartTime,
            x.ValidFrom,
            x.ValidTo
        });

        builder.HasIndex(x => new
        {
            x.SectionId,
            x.DayOfWeek,
            x.StartTime,
            x.ValidFrom,
            x.ValidTo
        });

        builder.HasIndex(x => new
        {
            x.RoomId,
            x.DayOfWeek,
            x.StartTime,
            x.ValidFrom,
            x.ValidTo
        });

        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.DayOfWeek
        });

        builder.HasIndex(x => new
        {
            x.DayOfWeek,
            x.DisplayOrder
        });

        builder.HasIndex(x => new
        {
            x.ValidFrom,
            x.ValidTo
        });

        builder.HasIndex(x => x.IsActive);
    }
}