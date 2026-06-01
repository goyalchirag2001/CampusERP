using CampusERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusERP.Infrastructure.Configurations;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Website)
            .HasMaxLength(500);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);
    }
}