using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class ChronicDiseaseConfiguration : IEntityTypeConfiguration<ChronicDisease>
{
    public void Configure(EntityTypeBuilder<ChronicDisease> builder)
    {
        builder.ToTable("ChronicDiseases");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DiagnosisDate)
            .IsRequired();

        builder.Property(e => e.Medications)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).IsRequired();

        // فهارس للأداء
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => new { e.PatientId, e.IsDeleted });

        // علاقة Many-to-One: مريض واحد ← عدة أمراض مزمنة
        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft Delete Filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}