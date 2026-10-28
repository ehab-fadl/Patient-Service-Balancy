using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class PatientReportConfiguration : IEntityTypeConfiguration<PatientReport>
{
    public void Configure(EntityTypeBuilder<PatientReport> builder)
    {
        builder.ToTable("PatientReports");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ReportType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ReportTitle).IsRequired().HasMaxLength(200);
        builder.Property(e => e.IsLegal).IsRequired();

        builder.Property(e => e.PatientNameSnapshot).IsRequired().HasMaxLength(150);
        builder.Property(e => e.PatientGenderSnapshot).HasMaxLength(20);
        builder.Property(e => e.PatientDiagnosisSnapshot).HasMaxLength(200);

        builder.Property(e => e.FormDataJson).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(e => e.Notes).HasMaxLength(2000);

        builder.Property(e => e.Status).IsRequired().HasMaxLength(30);
        builder.Property(e => e.DocumentDate).IsRequired();
        builder.Property(e => e.IsFavorite).IsRequired().HasDefaultValue(false);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.DoctorId);
        builder.HasIndex(e => new { e.PatientId, e.IsDeleted });
        builder.HasIndex(e => new { e.DoctorId, e.ReportType });

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}