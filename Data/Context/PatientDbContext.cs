using Microsoft.EntityFrameworkCore;
using PatientService.Models.Common;
using PatientService.Models.Entities;

namespace PatientService.Data.Context;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

    public DbSet<PatientPersonalInfo> PatientPersonalInfos => Set<PatientPersonalInfo>();
    public DbSet<PatientCode> PatientCodes => Set<PatientCode>();
    public DbSet<DepressionStory> DepressionStories => Set<DepressionStory>();
    public DbSet<ChronicDisease> ChronicDiseases => Set<ChronicDisease>();
    public DbSet<ChronicPsychDisease> ChronicPsychDiseases => Set<ChronicPsychDisease>();
    public DbSet<FamilyHistory> FamilyHistories => Set<FamilyHistory>();
    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();
    public DbSet<Gad7Assessment> Gad7Assessments => Set<Gad7Assessment>();
    public DbSet<Phq9Assessment> Phq9Assessments => Set<Phq9Assessment>();
    public DbSet<MdqAssessment> MdqAssessments => Set<MdqAssessment>();
    public DbSet<SdsAssessment> SdsAssessments => Set<SdsAssessment>();
    public DbSet<CssrsAssessment> CssrsAssessments => Set<CssrsAssessment>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Medication> Medications => Set<Medication>(); // ← جديد

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}