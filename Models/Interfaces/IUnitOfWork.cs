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
    ISideEffectRepository SideEffects { get; }
    IVisitRepository Visits { get; }
    ISubstanceUseRepository SubstanceUses { get; }
    IListeningSessionRepository ListeningSessions { get; }
    IPatientNoteRepository PatientNotes { get; }
    IDoctorNoteRepository DoctorNotes { get; }
    IDailyPatientMetricRepository DailyPatientMetrics { get; }
    IPatientReportRepository PatientReports { get; }
    IPatientDocumentRepository PatientDocuments { get; }
    ISystemAlertRepository SystemAlerts { get; }
    IAppointmentRepository Appointments { get; }

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