namespace PatientService.Models.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientPersonalInfoRepository PatientPersonalInfos { get; }
    IPatientCodeRepository PatientCodes { get; }
    IDepressionStoryRepository DepressionStories { get; }
    IChronicDiseaseRepository ChronicDiseases { get; }
    IChronicPsychDiseaseRepository ChronicPsychDiseases { get; }
    IFamilyHistoryRepository FamilyHistories { get; }
    IVitalSignRepository VitalSigns { get; } // ← جديد

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}