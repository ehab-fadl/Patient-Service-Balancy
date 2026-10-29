using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class SystemAlertConfiguration : IEntityTypeConfiguration<SystemAlert>
{
    public void Configure(EntityTypeBuilder<SystemAlert> builder)
    {
        builder.ToTable("SystemAlerts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.RuleId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Level).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Message).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ActionLabel).HasMaxLength(100);

        builder.Property(e => e.IsDismissed).IsRequired().HasDefaultValue(false);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => new { e.DoctorId, e.IsDismissed, e.IsDeleted });

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}