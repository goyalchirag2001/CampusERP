using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
        new Role
        {
            Id = SeedData.PlatformAdminRoleId,
            Name = RoleConstants.PlatformAdmin,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0,DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0,DateTimeKind.Utc)
        },

        new Role
        {
            Id = SeedData.InstitutionAdminRoleId,
            Name = RoleConstants.InstitutionAdmin,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },

        new Role
        {
            Id = SeedData.CampusAdminRoleId,
            Name = RoleConstants.CampusAdmin,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },

        new Role
        {
            Id = SeedData.TeacherRoleId,
            Name = RoleConstants.Teacher,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        },

        new Role
        {
            Id = SeedData.StudentRoleId,
            Name = RoleConstants.Student,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}