using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class DepressionStoryConfiguration : IEntityTypeConfiguration<DepressionStory>
{
    public void Configure(EntityTypeBuilder<DepressionStory> builder)
    {
        builder.ToTable("DepressionStories");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.Story).HasColumnType("nvarchar(max)");
        builder.Property(e => e.StoryVersion).IsRequired().HasDefaultValue(1);
        builder.Property(e => e.LastReviewedAt);

        // ✅ فهرس فريد على PatientId (لضمان One-to-One)
        builder.HasIndex(e => e.PatientId).IsUnique();

        // ✅ علاقة One-to-One: مريض واحد ← قصة واحدة
        builder.HasOne(e => e.Patient)
            .WithOne(p => p.DepressionStory)
            .HasForeignKey<DepressionStory>(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}