using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class TreatmentPlanConfiguration : IEntityTypeConfiguration<TreatmentPlan>
{
    public void Configure(EntityTypeBuilder<TreatmentPlan> builder)
    {
        builder.ToTable("TreatmentPlans");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(30);
        builder.Property(e => e.PlanDoctorName).HasMaxLength(100);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // حذف المشاكل عند حذف الخطة (Soft Delete Cascade لا يعمل تلقائياً، لذا نعتمد على الكود)
        builder.HasMany(e => e.Problems)
            .WithOne(p => p.TreatmentPlan)
            .HasForeignKey(p => p.TreatmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}