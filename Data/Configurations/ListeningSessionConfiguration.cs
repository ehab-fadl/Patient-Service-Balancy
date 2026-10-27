using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class ListeningSessionConfiguration : IEntityTypeConfiguration<ListeningSession>
{
    public void Configure(EntityTypeBuilder<ListeningSession> builder)
    {
        builder.ToTable("ListeningSessions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title).IsRequired().HasMaxLength(150);
        builder.Property(e => e.SessionDate).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasMaxLength(30);

        builder.Property(e => e.SessionType).HasMaxLength(50);
        builder.Property(e => e.TherapyType).HasMaxLength(100);
        builder.Property(e => e.Format).HasMaxLength(50);
        builder.Property(e => e.TherapistName).HasMaxLength(100);

        builder.Property(e => e.TopicsDiscussed).HasMaxLength(1000); // CSV
        builder.Property(e => e.TechniquesUsed).HasMaxLength(1000); // CSV
        builder.Property(e => e.RiskLevel).HasMaxLength(30);

        builder.Property(e => e.HomeworkAssigned).HasMaxLength(1000);
        builder.Property(e => e.NextSessionPlan).HasMaxLength(1000);

        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.Property(e => e.ClinicalNotes).HasMaxLength(2000);

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