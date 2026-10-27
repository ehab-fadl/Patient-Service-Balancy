using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class SubstanceUseConfiguration : IEntityTypeConfiguration<SubstanceUse>
{
    public void Configure(EntityTypeBuilder<SubstanceUse> builder)
    {
        builder.ToTable("SubstanceUses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Route).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Frequency).IsRequired().HasMaxLength(50);

        builder.Property(e => e.Amount).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Reason).HasMaxLength(100);
        builder.Property(e => e.CurrentStatus).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Severity).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Notes).HasMaxLength(500);

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