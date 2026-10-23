using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class TreatmentGoalHistoryConfiguration : IEntityTypeConfiguration<TreatmentGoalHistory>
{
    public void Configure(EntityTypeBuilder<TreatmentGoalHistory> builder)
    {
        builder.ToTable("TreatmentGoalHistories");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Value).HasMaxLength(200);
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.Property(e => e.RecordedAt).IsRequired();

        builder.Property(e => e.TreatmentGoalId).IsRequired();

        builder.HasIndex(e => e.TreatmentGoalId);
        builder.HasIndex(e => new { e.TreatmentGoalId, e.RecordedAt });

        builder.HasOne(e => e.TreatmentGoal)
            .WithMany(g => g.ProgressHistory)
            .HasForeignKey(e => e.TreatmentGoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}