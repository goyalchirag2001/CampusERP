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


        builder.Property(x => x.AdmissionNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Institution)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Student)
            .HasForeignKey<Student>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Enrollments)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CampusId,
            x.RollNumber
        }).IsUnique();
    }
}