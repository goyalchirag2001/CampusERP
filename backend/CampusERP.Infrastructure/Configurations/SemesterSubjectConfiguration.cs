using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class SemesterSubjectConfiguration: IEntityTypeConfiguration<SemesterSubject>
{
    public void Configure(EntityTypeBuilder<SemesterSubject> builder)
    {
        builder.ToTable("SemesterSubjects");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Semester)
            .WithMany(x => x.SemesterSubjects)
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subject)
            .WithMany(x => x.SemesterSubjects)
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.SemesterId,
            x.SubjectId
        }).IsUnique();
    }
}