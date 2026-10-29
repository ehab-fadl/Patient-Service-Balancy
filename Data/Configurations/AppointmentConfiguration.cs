using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ScheduledDate).IsRequired();
        builder.Property(e => e.ScheduledTime).HasMaxLength(10);

        builder.Property(e => e.Reason).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.Property(e => e.VisitType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.VisitLocation).IsRequired().HasMaxLength(50);

        builder.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
        builder.Property(e => e.CancellationReason).HasMaxLength(500);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => new { e.DoctorId, e.Status });
        builder.HasIndex(e => new { e.DoctorId, e.ScheduledDate });

        // علاقة مع المريض (Cascade - إذا حُذف المريض، حُذفت المواعيد)
        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // علاقة مع الزيارة (NoAction - لا نحذف الموعد عند حذف الزيارة)
        builder.HasOne(e => e.Visit)
            .WithMany()
            .HasForeignKey(e => e.VisitId)
            .OnDelete(DeleteBehavior.NoAction); // ← التعديل

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}