namespace PatientService.Models.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientPersonalInfoRepository PatientPersonalInfos { get; }
    IPatientCodeRepository PatientCodes { get; }
    IDepressionStoryRepository DepressionStories { get; }
    IChronicDiseaseRepository ChronicDiseases { get; }
    IChronicPsychDiseaseRepository ChronicPsychDiseases { get; }
    IFamilyHistoryRepository FamilyHistories { get; }
    IVitalSignRepository VitalSigns { get; }
    IGad7AssessmentRepository Gad7Assessments { get; }
    IPhq9AssessmentRepository Phq9Assessments { get; } // ← جديد

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}