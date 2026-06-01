using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Designation)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.Teachers)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Teacher)
            .HasForeignKey<Teacher>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Teachers)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.EmployeeCode
        }).IsUnique();
    }
}