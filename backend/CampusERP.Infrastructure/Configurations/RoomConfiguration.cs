using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Building)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Floor)
            .HasMaxLength(50);

        builder.Property(x => x.RoomNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RoomName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.RoomType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Capacity)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.HasProjector)
            .HasDefaultValue(false);

        builder.Property(x => x.HasSmartBoard)
            .HasDefaultValue(false);

        builder.Property(x => x.HasAirConditioning)
            .HasDefaultValue(false);

        builder.Property(x => x.HasComputers)
            .HasDefaultValue(false);

        builder.Property(x => x.HasInternet)
            .HasDefaultValue(false);

        builder.Property(x => x.IsAccessible)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.TimetableTemplates)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CalendarEvents)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.Building,
            x.RoomNumber
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.RoomName
        }).IsUnique();

        builder.HasIndex(x => new
        {
            x.CampusId,
            x.Building,
            x.Floor
        });

        builder.HasIndex(x => new
        {
            x.CampusId,
            x.RoomType
        });

        builder.HasIndex(x => x.IsActive);
    }
}