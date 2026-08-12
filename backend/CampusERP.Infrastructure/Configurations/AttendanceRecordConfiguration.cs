using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.MarkingMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.Property(x => x.IsMarked)
            .HasDefaultValue(false);

        builder.HasOne(x => x.AttendanceSession)
            .WithMany(x => x.AttendanceRecords)
            .HasForeignKey(x => x.AttendanceSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.AttendanceRecords)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MarkedByUser)
            .WithMany()
            .HasForeignKey(x => x.MarkedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        /*
         * A student can have only one attendance
         * record for a particular attendance session.
         */
        builder.HasIndex(x => new
        {
            x.AttendanceSessionId,
            x.StudentId
        })
        .IsUnique();

        builder.HasIndex(x => x.StudentId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.MarkingMethod);

        builder.HasIndex(x => x.IsMarked);

        builder.HasIndex(x => x.MarkedOn);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.StudentId
        });
    }
}