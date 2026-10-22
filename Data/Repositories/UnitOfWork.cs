using Microsoft.EntityFrameworkCore.Storage;
using PatientService.Data.Context;
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
    private IVitalSignRepository? _vitalSignRepository; // ← جديد

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
    public IVitalSignRepository VitalSigns => _vitalSignRepository ??= new VitalSignRepository(_context); // ← جديد

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