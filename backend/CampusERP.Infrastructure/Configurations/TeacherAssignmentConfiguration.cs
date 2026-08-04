using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class TeacherAssignmentConfiguration : IEntityTypeConfiguration<TeacherAssignment>
{
    public void Configure(EntityTypeBuilder<TeacherAssignment> builder)
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

        builder.HasOne(x => x.Section)
            .WithMany(x => x.TeacherAssignments)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicSession)
            .WithMany(x => x.TeacherAssignments)
            .HasForeignKey(x => x.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        // One teacher teaches one subject to one section in one academic session
        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.SectionId,
            x.SemesterSubjectId
        }).IsUnique();

        // Teacher workload
        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.AcademicSessionId
        });

        // Student timetable
        builder.HasIndex(x => new
        {
            x.SectionId,
            x.AcademicSessionId
        });

        // Subject allocation
        builder.HasIndex(x => new
        {
            x.SemesterSubjectId,
            x.AcademicSessionId
        });

        // Reporting
        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.SectionId
        });

        builder.HasIndex(x => x.TeacherId);

        builder.HasIndex(x => x.SectionId);

        builder.HasIndex(x => x.AcademicSessionId);

        builder.HasIndex(x => x.SemesterSubjectId);
    }
}