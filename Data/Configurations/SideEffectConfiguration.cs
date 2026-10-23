using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class SideEffectConfiguration : IEntityTypeConfiguration<SideEffect>
{
    public void Configure(EntityTypeBuilder<SideEffect> builder)
    {
        builder.ToTable("SideEffects");
        builder.HasKey(e => e.Id);

        // ✅ ضمان التعامل مع Guid بشكل صحيح
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.MedicationName).IsRequired().HasMaxLength(120);
        builder.Property(e => e.Symptom).IsRequired().HasMaxLength(300);
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