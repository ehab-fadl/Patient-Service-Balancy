using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("Medications");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(120);
        builder.Property(e => e.Dose).IsRequired().HasMaxLength(80);
        builder.Property(e => e.Frequency).IsRequired().HasMaxLength(50);
        builder.Property(e => e.IntakePeriod).IsRequired().HasMaxLength(30);
        builder.Property(e => e.Route).IsRequired().HasMaxLength(50);

        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate);

        builder.Property(e => e.Status).IsRequired().HasMaxLength(30);
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