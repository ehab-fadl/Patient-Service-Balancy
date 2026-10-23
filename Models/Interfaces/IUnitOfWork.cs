using PatientService.Models.Entities;

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
    IPhq9AssessmentRepository Phq9Assessments { get; }
    IMdqAssessmentRepository MdqAssessments { get; }
    ISdsAssessmentRepository SdsAssessments { get; }
    ICssrsAssessmentRepository CssrsAssessments { get; }
    IDiagnosisRepository Diagnoses { get; }
    IMedicationRepository Medications { get; }
    ISideEffectRepository SideEffects { get; } // ← جديد

    // Treatment Plans Tree
    ITreatmentPlanRepository TreatmentPlans { get; }
    IGenericRepository<TreatmentProblem> TreatmentProblems { get; }
    IGenericRepository<TreatmentGoal> TreatmentGoals { get; }
    IGenericRepository<TreatmentMean> TreatmentMeans { get; }
    IGenericRepository<TreatmentGoalHistory> TreatmentGoalHistories { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}