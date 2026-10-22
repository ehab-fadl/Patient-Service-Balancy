using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class FamilyHistoryConfiguration : IEntityTypeConfiguration<FamilyHistory>
{
    public void Configure(EntityTypeBuilder<FamilyHistory> builder)
    {
        builder.ToTable("FamilyHistories");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Relation).IsRequired().HasMaxLength(50);
        builder.Property(e => e.RelativeName).HasMaxLength(100);
        builder.Property(e => e.Status).HasMaxLength(20);

        builder.Property(e => e.Severity).IsRequired().HasMaxLength(20);
        builder.Property(e => e.ConditionType).IsRequired().HasMaxLength(20);

        builder.Property(e => e.Conditions).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(500);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => new { e.PatientId, e.IsDeleted });

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}