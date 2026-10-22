using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class VitalSignConfiguration : IEntityTypeConfiguration<VitalSign>
{
    public void Configure(EntityTypeBuilder<VitalSign> builder)
    {
        builder.ToTable("VitalSigns");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.VitalType).IsRequired().HasMaxLength(30);
        builder.Property(e => e.MeasurementDate).IsRequired();

        builder.Property(e => e.Value1).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(e => e.Value2).HasColumnType("decimal(18,2)");

        builder.Property(e => e.Status).IsRequired().HasMaxLength(20);
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