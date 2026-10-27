using Microsoft.EntityFrameworkCore.Storage;
using PatientService.Data.Context;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PatientDbContext _context;
    private IDbContextTransaction? _transaction;

    private IPatientPersonalInfoRepository? _patientPersonalInfoRepository;
    private IPatientCodeRepository? _patientCodeRepository;
    private IDepressionStoryRepository? _depressionStoryRepository;
    private IChronicDiseaseRepository? _chronicDiseaseRepository;
    private IChronicPsychDiseaseRepository? _chronicPsychDiseaseRepository;
    private IFamilyHistoryRepository? _familyHistoryRepository;
    private IVitalSignRepository? _vitalSignRepository;
    private IGad7AssessmentRepository? _gad7AssessmentRepository;
    private IPhq9AssessmentRepository? _phq9AssessmentRepository;
    private IMdqAssessmentRepository? _mdqAssessmentRepository;
    private ISdsAssessmentRepository? _sdsAssessmentRepository;
    private ICssrsAssessmentRepository? _cssrsAssessmentRepository;
    private IDiagnosisRepository? _diagnosisRepository;
    private IMedicationRepository? _medicationRepository;
    private ISideEffectRepository? _sideEffectRepository;
    private IVisitRepository? _visitRepository;
    private ISubstanceUseRepository? _substanceUseRepository;
    private IListeningSessionRepository? _listeningSessionRepository;
    private IPatientNoteRepository? _patientNoteRepository;
    private IDoctorNoteRepository? _doctorNoteRepository; // ← جديد

    private ITreatmentPlanRepository? _treatmentPlanRepository;
    private IGenericRepository<TreatmentProblem>? _treatmentProblemRepository;
    private IGenericRepository<TreatmentGoal>? _treatmentGoalRepository;
    private IGenericRepository<TreatmentMean>? _treatmentMeanRepository;
    private IGenericRepository<TreatmentGoalHistory>? _treatmentGoalHistoryRepository;

    public UnitOfWork(PatientDbContext context)
    {
        _context = context;
    }

    public IPatientPersonalInfoRepository PatientPersonalInfos => _patientPersonalInfoRepository ??= new PatientPersonalInfoRepository(_context);
    public IPatientCodeRepository PatientCodes => _patientCodeRepository ??= new PatientCodeRepository(_context);
    public IDepressionStoryRepository DepressionStories => _depressionStoryRepository ??= new DepressionStoryRepository(_context);
    public IChronicDiseaseRepository ChronicDiseases => _chronicDiseaseRepository ??= new ChronicDiseaseRepository(_context);
    public IChronicPsychDiseaseRepository ChronicPsychDiseases => _chronicPsychDiseaseRepository ??= new ChronicPsychDiseaseRepository(_context);
    public IFamilyHistoryRepository FamilyHistories => _familyHistoryRepository ??= new FamilyHistoryRepository(_context);
    public IVitalSignRepository VitalSigns => _vitalSignRepository ??= new VitalSignRepository(_context);
    public IGad7AssessmentRepository Gad7Assessments => _gad7AssessmentRepository ??= new Gad7AssessmentRepository(_context);
    public IPhq9AssessmentRepository Phq9Assessments => _phq9AssessmentRepository ??= new Phq9AssessmentRepository(_context);
    public IMdqAssessmentRepository MdqAssessments => _mdqAssessmentRepository ??= new MdqAssessmentRepository(_context);
    public ISdsAssessmentRepository SdsAssessments => _sdsAssessmentRepository ??= new SdsAssessmentRepository(_context);
    public ICssrsAssessmentRepository CssrsAssessments => _cssrsAssessmentRepository ??= new CssrsAssessmentRepository(_context);
    public IDiagnosisRepository Diagnoses => _diagnosisRepository ??= new DiagnosisRepository(_context);
    public IMedicationRepository Medications => _medicationRepository ??= new MedicationRepository(_context);
    public ISideEffectRepository SideEffects => _sideEffectRepository ??= new SideEffectRepository(_context);
    public IVisitRepository Visits => _visitRepository ??= new VisitRepository(_context);
    public ISubstanceUseRepository SubstanceUses => _substanceUseRepository ??= new SubstanceUseRepository(_context);
    public IListeningSessionRepository ListeningSessions => _listeningSessionRepository ??= new ListeningSessionRepository(_context);
    public IPatientNoteRepository PatientNotes => _patientNoteRepository ??= new PatientNoteRepository(_context);
    public IDoctorNoteRepository DoctorNotes => _doctorNoteRepository ??= new DoctorNoteRepository(_context); // ← جديد

    public ITreatmentPlanRepository TreatmentPlans => _treatmentPlanRepository ??= new TreatmentPlanRepository(_context);
    public IGenericRepository<TreatmentProblem> TreatmentProblems => _treatmentProblemRepository ??= new GenericRepository<TreatmentProblem>(_context);
    public IGenericRepository<TreatmentGoal> TreatmentGoals => _treatmentGoalRepository ??= new GenericRepository<TreatmentGoal>(_context);
    public IGenericRepository<TreatmentMean> TreatmentMeans => _treatmentMeanRepository ??= new GenericRepository<TreatmentMean>(_context);
    public IGenericRepository<TreatmentGoalHistory> TreatmentGoalHistories => _treatmentGoalHistoryRepository ??= new GenericRepository<TreatmentGoalHistory>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}