using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RollNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Batch)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AdmissionDate)
            .IsRequired();

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Student)
            .HasForeignKey<Student>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.RollNumber
        }).IsUnique();
    }
}