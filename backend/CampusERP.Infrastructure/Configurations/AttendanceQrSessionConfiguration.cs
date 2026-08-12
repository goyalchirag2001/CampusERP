using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class AttendanceQrSessionConfiguration: IEntityTypeConfiguration<AttendanceQrSession>
{
    public void Configure(EntityTypeBuilder<AttendanceQrSession> builder)
    {
        builder.ToTable("AttendanceQrSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ValidFrom)
            .IsRequired();

        builder.Property(x => x.ExpiresOn)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(false);

        builder.HasOne(x => x.AttendanceSession)
            .WithMany()
            .HasForeignKey(x => x.AttendanceSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.AttendanceSessionId,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.ExpiresOn,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.AttendanceSessionId
        });
    }
}