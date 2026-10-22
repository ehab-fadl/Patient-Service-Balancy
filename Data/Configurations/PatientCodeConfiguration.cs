using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class PatientCodeConfiguration : IEntityTypeConfiguration<PatientCode>
{
    public void Configure(EntityTypeBuilder<PatientCode> builder)
    {
        builder.ToTable("PatientCodes");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.GeneratedAt).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();

        // ✅ فهارس فريدة
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.PatientId).IsUnique();

        // ✅ علاقة One-to-One: مريض واحد ← كود واحد
        builder.HasOne(e => e.Patient)
            .WithOne(p => p.PatientCode)
            .HasForeignKey<PatientCode>(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}