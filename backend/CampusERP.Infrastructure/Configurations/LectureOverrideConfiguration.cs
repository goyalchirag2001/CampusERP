using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class LectureOverrideConfiguration: IEntityTypeConfiguration<LectureOverride>
{
    public void Configure(EntityTypeBuilder<LectureOverride> builder)
    {
        builder.ToTable("LectureOverrides");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OverrideType)
            .HasConversion<int>();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);

        builder.Property(x => x.GenerateAttendance)
            .HasDefaultValue(true);

        builder.Property(x => x.IsSystemGenerated)
            .HasDefaultValue(false);

        builder.Property(x => x.IsApproved)
            .HasDefaultValue(false);

        builder.Property(x => x.Version)
            .HasDefaultValue(1);

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.LectureOverrides)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.LectureOverrides)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicSession)
            .WithMany(x => x.LectureOverrides)
            .HasForeignKey(x => x.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TimetableTemplate)
            .WithMany(x => x.LectureOverrides)
            .HasForeignKey(x => x.TimetableTemplateId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false); ;

        builder.HasOne(x => x.CalendarEvent)
            .WithMany(x => x.LectureOverrides)
            .HasForeignKey(x => x.CalendarEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OriginalTeacher)
            .WithMany(x => x.OriginalLectureOverrides)
            .HasForeignKey(x => x.OriginalTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewTeacher)
            .WithMany(x => x.NewLectureOverrides)
            .HasForeignKey(x => x.NewTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OriginalRoom)
            .WithMany(x => x.OriginalLectureOverrides)
            .HasForeignKey(x => x.OriginalRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewRoom)
            .WithMany(x => x.NewLectureOverrides)
            .HasForeignKey(x => x.NewRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedByUser)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.OverrideDate
        });

        builder.HasIndex(x => new
        {
            x.TimetableTemplateId,
            x.OverrideDate
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.CalendarEventId
        });

        builder.HasIndex(x => new
        {
            x.NewTeacherId,
            x.OverrideDate
        });

        builder.HasIndex(x => new
        {
            x.NewRoomId,
            x.OverrideDate
        });

        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.OverrideDate,
            x.OverrideType
        });

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.OverrideDate
        });

        builder.HasIndex(x => new
        {
            x.OriginalTeacherId,
            x.OverrideDate
        });

        builder.HasIndex(x => new
        {
            x.OriginalRoomId,
            x.OverrideDate
        });

        builder.HasIndex(x => x.IsApproved);

        builder.HasIndex(x => x.IsSystemGenerated);
    }
}