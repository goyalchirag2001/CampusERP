using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class TeacherAssignmentConfiguration: IEntityTypeConfiguration<TeacherAssignment>
{
    public void Configure(
        EntityTypeBuilder<TeacherAssignment> builder)
    {
        builder.ToTable("TeacherAssignments");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Teacher)
            .WithMany(x => x.TeacherAssignments)
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SemesterSubject)
            .WithMany(x => x.TeacherAssignments)
            .HasForeignKey(x => x.SemesterSubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.SemesterSubjectId
        }).IsUnique();
    }
}