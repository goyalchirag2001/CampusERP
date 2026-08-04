using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class AcademicConfigurationConfiguration : IEntityTypeConfiguration<AcademicConfiguration>
{
    public void Configure(EntityTypeBuilder<AcademicConfiguration> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AcademicTermsPerSession)
            .HasDefaultValue(2);

        builder.Property(x => x.MinimumAttendancePercentage)
            .HasDefaultValue(75);

        builder.Property(x => x.AutoPromoteEnabled)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowAttendanceEditing)
            .HasDefaultValue(true);

        builder.Property(x => x.AttendanceEditWindowDays)
            .HasDefaultValue(7);

        builder.Property(x => x.AutoGenerateAttendanceSessions)
            .HasDefaultValue(true);

        builder.Property(x => x.AutoGenerateAttendanceRecords)
            .HasDefaultValue(true);

        builder.Property(x => x.AttendanceLockAfterDays)
            .HasDefaultValue(7);

        builder.Property(x => x.AllowTeacherAttendanceUnlock)
            .HasDefaultValue(false);

        builder.Property(x => x.LateThresholdMinutes)
            .HasDefaultValue(10);

        builder.Property(x => x.MedicalLeaveCountsAsPresent)
            .HasDefaultValue(false);

        builder.Property(x => x.OnDutyCountsAsPresent)
            .HasDefaultValue(true);

        builder.Property(x => x.AllowStudentAttendanceCorrection)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Institution)
            .WithMany()
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Campus)
            .WithMany()
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId
        }).IsUnique();
    }
}