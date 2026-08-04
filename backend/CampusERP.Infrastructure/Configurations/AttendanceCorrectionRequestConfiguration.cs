using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class AttendanceCorrectionRequestConfiguration : IEntityTypeConfiguration<AttendanceCorrectionRequest>
{
    public void Configure(EntityTypeBuilder<AttendanceCorrectionRequest> builder)
    {
        builder.ToTable("AttendanceCorrectionRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.RequestedStatus)
            .HasConversion<int>();

        builder.Property(x => x.OriginalStatus)
            .HasConversion<int>();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.ReviewRemarks)
            .HasMaxLength(1000);

        builder.Property(x => x.AttachmentPath)
            .HasMaxLength(500);

        builder.Property(x => x.IsProcessed)
            .HasDefaultValue(false);

        builder.HasOne(x => x.AttendanceRecord)
            .WithMany(x => x.CorrectionRequests)
            .HasForeignKey(x => x.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.AttendanceCorrectionRequests)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany(x => x.ReviewedAttendanceCorrectionRequests)
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AttendanceRecordId);

        builder.HasIndex(x => x.StudentId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Reason);

        builder.HasIndex(x => x.ReviewedByUserId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.IsProcessed);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.StudentId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.AttendanceRecordId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.AttendanceRecordId,
            x.StudentId
        });

        builder.HasIndex(x => new
        {
            x.CreatedAt
        });
    }
}