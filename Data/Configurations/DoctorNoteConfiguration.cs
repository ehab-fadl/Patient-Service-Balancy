using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class DoctorNoteConfiguration : IEntityTypeConfiguration<DoctorNote>
{
    public void Configure(EntityTypeBuilder<DoctorNote> builder)
    {
        builder.ToTable("DoctorNotes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title).IsRequired().HasMaxLength(150);
        builder.Property(e => e.NoteDate).IsRequired();
        builder.Property(e => e.NoteType).IsRequired().HasMaxLength(30);
        builder.Property(e => e.Category).HasMaxLength(50);
        builder.Property(e => e.Author).IsRequired().HasMaxLength(100);
        builder.Property(e => e.AuthorRole).IsRequired().HasMaxLength(50);

        builder.Property(e => e.Priority).HasMaxLength(20);
        builder.Property(e => e.RiskLevel).HasMaxLength(20);
        builder.Property(e => e.Icd10).HasMaxLength(100);
        builder.Property(e => e.Visibility).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Symptoms).HasMaxLength(1000); // CSV

        builder.Property(e => e.LinkType).HasMaxLength(20);
        builder.Property(e => e.LinkItemId);

        builder.Property(e => e.Content).IsRequired().HasMaxLength(3000);
        builder.Property(e => e.NextPlan).HasMaxLength(1000);

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