using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class CssrsAssessmentConfiguration : IEntityTypeConfiguration<CssrsAssessment>
{
    public void Configure(EntityTypeBuilder<CssrsAssessment> builder)
    {
        builder.ToTable("CssrsAssessments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TotalScore).IsRequired();
        builder.Property(e => e.Severity).IsRequired().HasMaxLength(30);
        builder.Property(e => e.IsCritical).IsRequired();
        builder.Property(e => e.AnswersJson).HasColumnType("nvarchar(max)");

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