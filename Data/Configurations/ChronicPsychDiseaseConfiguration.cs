using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class ChronicPsychDiseaseConfiguration : IEntityTypeConfiguration<ChronicPsychDisease>
{
    public void Configure(EntityTypeBuilder<ChronicPsychDisease> builder)
    {
        builder.ToTable("ChronicPsychDiseases");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.DiagnosisDate).IsRequired();

        builder.Property(e => e.Hospitalization).IsRequired().HasMaxLength(3);
        builder.Property(e => e.Medications).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Severity).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Response).IsRequired().HasMaxLength(30);
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