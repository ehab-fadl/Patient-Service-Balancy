using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class TreatmentProblemConfiguration : IEntityTypeConfiguration<TreatmentProblem>
{
    public void Configure(EntityTypeBuilder<TreatmentProblem> builder)
    {
        builder.ToTable("TreatmentProblems");
        builder.HasKey(e => e.Id);

        // ✅ أضف هذا السطر لكل كيان تقوم بتعيين Id له يدوياً
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Text).IsRequired().HasMaxLength(500);

        builder.HasOne(e => e.TreatmentPlan)
            .WithMany(p => p.Problems)
            .HasForeignKey(e => e.TreatmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}