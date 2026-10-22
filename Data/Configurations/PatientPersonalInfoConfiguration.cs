using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientService.Models.Entities;

namespace PatientService.Data.Configurations;

public class PatientPersonalInfoConfiguration : IEntityTypeConfiguration<PatientPersonalInfo>
{
    public void Configure(EntityTypeBuilder<PatientPersonalInfo> builder)
    {
        builder.ToTable("PatientPersonalInfos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Gender).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(300);
        builder.Property(e => e.City).HasMaxLength(60);
        builder.Property(e => e.MaritalStatus).HasMaxLength(20);

        builder.HasIndex(e => e.Email);

        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.Property(e => e.DoctorId).IsRequired();
        builder.HasIndex(e => e.DoctorId);  
    }
}