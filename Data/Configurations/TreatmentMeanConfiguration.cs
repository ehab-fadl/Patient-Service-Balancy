using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class TreatmentMeanConfiguration : IEntityTypeConfiguration<TreatmentMean>
{
    public void Configure(EntityTypeBuilder<TreatmentMean> builder)
    {
        builder.ToTable("TreatmentMeans");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text).IsRequired().HasMaxLength(500);
        builder.Property(e => e.TreatmentGoalId).IsRequired();

        builder.HasIndex(e => e.TreatmentGoalId);

        builder.HasOne(e => e.TreatmentGoal)
            .WithMany(g => g.Means)
            .HasForeignKey(e => e.TreatmentGoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}