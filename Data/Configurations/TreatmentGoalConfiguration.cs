using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class TreatmentGoalConfiguration : IEntityTypeConfiguration<TreatmentGoal>
{
    public void Configure(EntityTypeBuilder<TreatmentGoal> builder)
    {
        builder.ToTable("TreatmentGoals");
        builder.HasKey(e => e.Id);

        // ✅ هذا هو السطر الذي سيحل المشكلة
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Text).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Unit).HasMaxLength(50);
        builder.Property(e => e.Baseline).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TargetValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Status).IsRequired().HasMaxLength(30).HasDefaultValue("pending");

        builder.HasOne(e => e.TreatmentProblem)
            .WithMany(p => p.Goals)
            .HasForeignKey(e => e.TreatmentProblemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}