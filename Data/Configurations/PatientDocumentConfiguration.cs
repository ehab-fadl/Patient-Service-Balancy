using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class PatientDocumentConfiguration : IEntityTypeConfiguration<PatientDocument>
{
    public void Configure(EntityTypeBuilder<PatientDocument> builder)
    {
        builder.ToTable("PatientDocuments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);
        builder.Property(e => e.SourceType).IsRequired().HasMaxLength(20);

        builder.Property(e => e.Issuer).HasMaxLength(150);
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.Property(e => e.RefNo).HasMaxLength(100);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.Tags).HasMaxLength(500);

        builder.Property(e => e.FileName).HasMaxLength(255);
        builder.Property(e => e.FilePath).HasMaxLength(500);
        builder.Property(e => e.FileSize).IsRequired();

        builder.Property(e => e.DocumentDate).IsRequired();
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