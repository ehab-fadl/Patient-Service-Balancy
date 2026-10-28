using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class DailyPatientMetricConfiguration : IEntityTypeConfiguration<DailyPatientMetric>
{
    public void Configure(EntityTypeBuilder<DailyPatientMetric> builder)
    {
        builder.ToTable("DailyPatientMetrics");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.LogDate).IsRequired();
        builder.Property(e => e.SleepHours).HasColumnType("decimal(4,1)"); // 7.5 مثلاً

        builder.Property(e => e.PatientId).IsRequired();

        // فهرس مركب يمنع تكرار قراءتين في نفس اليوم لنفس المريض
        builder.HasIndex(e => new { e.PatientId, e.LogDate }).IsUnique();

        builder.HasIndex(e => new { e.PatientId, e.IsDeleted });

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}