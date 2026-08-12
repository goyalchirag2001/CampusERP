using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class AttendanceSessionConfiguration : IEntityTypeConfiguration<AttendanceSession>
{
    public void Configure(EntityTypeBuilder<AttendanceSession> builder)
    {
        builder.ToTable("AttendanceSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LectureType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Source)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsAttendanceMarked)
            .HasDefaultValue(false);

        builder.Property(x => x.IsLocked)
            .HasDefaultValue(false);

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Institution)
            .WithMany()
            .HasForeignKey(x => x.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Campus)
            .WithMany()
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicSession)
            .WithMany(x => x.AttendanceSessions)
            .HasForeignKey(x => x.AcademicSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TeacherAssignment)
            .WithMany()
            .HasForeignKey(x => x.TeacherAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TimetableTemplate)
            .WithMany()
            .HasForeignKey(x => x.TimetableTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LectureOverride)
            .WithMany()
            .HasForeignKey(x => x.LectureOverrideId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SemesterSubject)
            .WithMany()
            .HasForeignKey(x => x.SemesterSubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Section)
            .WithMany()
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.QrSessions)
            .WithOne(x => x.AttendanceSession)
            .HasForeignKey(x => x.AttendanceSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LockedByUser)
            .WithMany()
            .HasForeignKey(x => x.LockedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        /*
         * Fast lookup when loading a teacher's
         * attendance sessions.
         */
        builder.HasIndex(x => new
        {
            x.TeacherId,
            x.AttendanceDate
        });

        /*
         * Fast section attendance lookup.
         */
        builder.HasIndex(x => new
        {
            x.SectionId,
            x.AttendanceDate
        });

        /*
         * Student attendance reports eventually
         * traverse through section/session.
         */
        builder.HasIndex(x => new
        {
            x.AcademicSessionId,
            x.SectionId,
            x.AttendanceDate
        });

        builder.HasIndex(x => new
        {
            x.TimetableTemplateId,
            x.AttendanceDate
        });

        builder.HasIndex(x => new
        {
            x.LectureOverrideId,
            x.AttendanceDate
        });

        builder.HasIndex(x => new
        {
            x.Status,
            x.AttendanceDate
        });

        builder.HasIndex(x => new
        {
            x.InstitutionId,
            x.CampusId,
            x.AttendanceDate
        });
    }
}