using Microsoft.Extensions.Logging;
using PatientService.Helpers;
using PatientService.Models.Interfaces;
using PatientService.Services.DTOs.ETL;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class EtlService : IEtlService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EtlService> _logger;

    public EtlService(IUnitOfWork unitOfWork, ILogger<EtlService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private static EtlResponse<T> Success<T>(T data, int page, int pageSize, int totalCount)
    {
        return new EtlResponse<T>
        {
            Success = true,
            Data = data,
            Pagination = new PaginationMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            },
            SyncMetadata = new SyncMetadata
            {
                DataAsOf = DateTime.UtcNow,
                DeidentificationVersion = "1.1",
                ETLSessionId = Guid.NewGuid().ToString("N")[..8]
            }
        };
    }

    // 1. Health
    public Task<EtlResponse<EtlHealthDto>> GetHealthAsync()
    {
        var health = new EtlHealthDto
        {
            Status = "healthy",
            ServerTimeUtc = DateTime.UtcNow,
            DbVersion = "1.0",
            LastSuccessfulSync = DateTime.UtcNow.AddDays(-1),
            SupportedEntities = new List<string>
            {
                "patients", "assessments", "medications", "diagnoses",
                "chronic_conditions", "family_history", "substance_use",
                "listening_sessions", "visits", "treatment_plan_goals", "vital_signs"
            }
        };
        return Task.FromResult(Success(health, 1, 1, 1));
    }

    // 2. Patients
    public async Task<EtlResponse<List<EtlPatientDto>>> GetPatientsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var patients = await _unitOfWork.PatientPersonalInfos.FindAsync(p => p.UpdatedAt >= cutoff || p.CreatedAt >= cutoff);
        var total = patients.Count();
        var paginated = patients.OrderBy(p => p.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(p => new EtlPatientDto
        {
            PatientHash = DeidentificationHelper.HashId(p.Id),
            AgeGroup = DeidentificationHelper.GeneralizeAge(p.DateOfBirth),
            Gender = DeidentificationHelper.MapGender(p.Gender),
            Region = DeidentificationHelper.MapRegion(p.City),
            MaritalStatus = DeidentificationHelper.MapMaritalStatus(p.MaritalStatus),
            CreatedQuarter = DeidentificationHelper.GeneralizeQuarter(p.CreatedAt),
            UpdatedAt = p.UpdatedAt ?? p.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 3. Assessments
    public async Task<EtlResponse<List<EtlAssessmentDto>>> GetAssessmentsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var allAssessments = new List<EtlAssessmentDto>();

        var gad7 = await _unitOfWork.Gad7Assessments.FindAsync(a => a.CreatedAt >= cutoff);
        allAssessments.AddRange(gad7.Select(a => new EtlAssessmentDto { AssessmentHash = DeidentificationHelper.HashId(a.Id), PatientHash = DeidentificationHelper.HashId(a.PatientId), AssessmentType = "GAD7", TotalScore = a.TotalScore, Severity = a.Severity, AssessmentQuarter = DeidentificationHelper.GeneralizeQuarter(a.CreatedAt), CreatedAt = a.CreatedAt }));

        var phq9 = await _unitOfWork.Phq9Assessments.FindAsync(a => a.CreatedAt >= cutoff);
        allAssessments.AddRange(phq9.Select(a => new EtlAssessmentDto { AssessmentHash = DeidentificationHelper.HashId(a.Id), PatientHash = DeidentificationHelper.HashId(a.PatientId), AssessmentType = "PHQ9", TotalScore = a.TotalScore, Severity = a.Severity, AssessmentQuarter = DeidentificationHelper.GeneralizeQuarter(a.CreatedAt), CreatedAt = a.CreatedAt }));

        var mdq = await _unitOfWork.MdqAssessments.FindAsync(a => a.CreatedAt >= cutoff);
        allAssessments.AddRange(mdq.Select(a => new EtlAssessmentDto { AssessmentHash = DeidentificationHelper.HashId(a.Id), PatientHash = DeidentificationHelper.HashId(a.PatientId), AssessmentType = "MDQ", TotalScore = a.TotalScore, Severity = a.Severity, AssessmentQuarter = DeidentificationHelper.GeneralizeQuarter(a.CreatedAt), CreatedAt = a.CreatedAt }));

        var sds = await _unitOfWork.SdsAssessments.FindAsync(a => a.CreatedAt >= cutoff);
        allAssessments.AddRange(sds.Select(a => new EtlAssessmentDto { AssessmentHash = DeidentificationHelper.HashId(a.Id), PatientHash = DeidentificationHelper.HashId(a.PatientId), AssessmentType = "SDS", TotalScore = a.TotalScore, Severity = a.Severity, AssessmentQuarter = DeidentificationHelper.GeneralizeQuarter(a.CreatedAt), CreatedAt = a.CreatedAt }));

        var cssrs = await _unitOfWork.CssrsAssessments.FindAsync(a => a.CreatedAt >= cutoff);
        allAssessments.AddRange(cssrs.Select(a => new EtlAssessmentDto { AssessmentHash = DeidentificationHelper.HashId(a.Id), PatientHash = DeidentificationHelper.HashId(a.PatientId), AssessmentType = "CSSRS", TotalScore = a.TotalScore, Severity = a.Severity, AssessmentQuarter = DeidentificationHelper.GeneralizeQuarter(a.CreatedAt), CreatedAt = a.CreatedAt }));

        var total = allAssessments.Count;
        var paginated = allAssessments.OrderBy(a => a.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Success(paginated, page, pageSize, total);
    }

    // 4. Medications
    public async Task<EtlResponse<List<EtlMedicationDto>>> GetMedicationsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var medications = await _unitOfWork.Medications.FindAsync(m => m.CreatedAt >= cutoff || (m.UpdatedAt != null && m.UpdatedAt >= cutoff));
        var total = medications.Count();
        var paginated = medications.OrderBy(m => m.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(m => new EtlMedicationDto
        {
            MedicationHash = DeidentificationHelper.HashId(m.Id),
            PatientHash = DeidentificationHelper.HashId(m.PatientId),
            MedicationClass = DeidentificationHelper.MapMedicationClass(m.Name),
            DoseCategory = DeidentificationHelper.GeneralizeDoseCategory(m.Dose),
            Frequency = DeidentificationHelper.MapFrequency(m.Frequency),
            Status = DeidentificationHelper.MapMedicationStatus(m.Status),
            StartQuarter = DeidentificationHelper.GeneralizeQuarter(m.StartDate),
            DurationDays = DeidentificationHelper.CalculateDurationDays(m.StartDate, m.EndDate),
            CreatedAt = m.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 5. Diagnoses
    public async Task<EtlResponse<List<EtlDiagnosisDto>>> GetDiagnosesAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var diagnoses = await _unitOfWork.Diagnoses.FindAsync(d => d.CreatedAt >= cutoff);
        var total = diagnoses.Count();
        var paginated = diagnoses.OrderBy(d => d.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(d => new EtlDiagnosisDto
        {
            DiagnosisHash = DeidentificationHelper.HashId(d.Id),
            PatientHash = DeidentificationHelper.HashId(d.PatientId),
            MddType = d.MainType,
            MddSubtype = d.SubType,
            DiagnosisQuarter = DeidentificationHelper.GeneralizeQuarter(d.DiagnosisDate),
            Severity = DeidentificationHelper.MapDiagnosisSeverity(d.Severity),
            CreatedAt = d.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 6. Chronic Conditions
    public async Task<EtlResponse<List<EtlChronicConditionDto>>> GetChronicConditionsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var allConditions = new List<EtlChronicConditionDto>();

        var physical = await _unitOfWork.ChronicDiseases.FindAsync(c => c.CreatedAt >= cutoff);
        allConditions.AddRange(physical.Select(c => new EtlChronicConditionDto { ConditionHash = DeidentificationHelper.HashId(c.Id), PatientHash = DeidentificationHelper.HashId(c.PatientId), ConditionCategory = DeidentificationHelper.MapConditionType(c.Name), ConditionType = "chronic_physical", Severity = null, TreatmentResponse = null, HasHospitalization = false, DiagnosisQuarter = DeidentificationHelper.GeneralizeQuarter(c.DiagnosisDate), CreatedAt = c.CreatedAt }));

        var psychiatric = await _unitOfWork.ChronicPsychDiseases.FindAsync(c => c.CreatedAt >= cutoff);
        allConditions.AddRange(psychiatric.Select(c => new EtlChronicConditionDto { ConditionHash = DeidentificationHelper.HashId(c.Id), PatientHash = DeidentificationHelper.HashId(c.PatientId), ConditionCategory = DeidentificationHelper.MapConditionType(c.Name), ConditionType = "psychiatric", Severity = DeidentificationHelper.MapDiagnosisSeverity(c.Severity), TreatmentResponse = DeidentificationHelper.MapTreatmentResponse(c.Response), HasHospitalization = c.Hospitalization == "نعم", DiagnosisQuarter = DeidentificationHelper.GeneralizeQuarter(c.DiagnosisDate), CreatedAt = c.CreatedAt }));

        var total = allConditions.Count;
        var paginated = allConditions.OrderBy(c => c.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Success(paginated, page, pageSize, total);
    }

    // 7. Family History
    public async Task<EtlResponse<List<EtlFamilyHistoryDto>>> GetFamilyHistoryAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var history = await _unitOfWork.FamilyHistories.FindAsync(f => f.CreatedAt >= cutoff);
        var total = history.Count();
        var paginated = history.OrderBy(f => f.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(f => new EtlFamilyHistoryDto
        {
            FamilyHistoryHash = DeidentificationHelper.HashId(f.Id),
            PatientHash = DeidentificationHelper.HashId(f.PatientId),
            RelationType = DeidentificationHelper.MapRelation(f.Relation),
            ConditionType = f.ConditionType,
            Severity = f.Severity,
            HasCondition = !string.IsNullOrWhiteSpace(f.Conditions),
            CreatedAt = f.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 8. Substance Use
    public async Task<EtlResponse<List<EtlSubstanceUseDto>>> GetSubstanceUseAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var substances = await _unitOfWork.SubstanceUses.FindAsync(s => s.CreatedAt >= cutoff);
        var total = substances.Count();
        var paginated = substances.OrderBy(s => s.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(s => new EtlSubstanceUseDto
        {
            SubstanceUseHash = DeidentificationHelper.HashId(s.Id),
            PatientHash = DeidentificationHelper.HashId(s.PatientId),
            SubstanceCategory = DeidentificationHelper.MapSubstanceType(s.Type),
            Severity = DeidentificationHelper.MapDiagnosisSeverity(s.Severity),
            CurrentStatus = DeidentificationHelper.MapSubstanceStatus(s.CurrentStatus),
            CravingLevel = s.CravingLevel,
            HasPhysicalDependence = s.PhysicalDependence,
            HasPsychologicalDependence = s.PsychologicalDependence,
            HasImpactWork = s.ImpactWork,
            HasImpactHealth = s.ImpactHealth,
            QuitAttempts = s.QuitAttempts,
            CreatedAt = s.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 9. Listening Sessions
    public async Task<EtlResponse<List<EtlListeningSessionDto>>> GetListeningSessionsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var sessions = await _unitOfWork.ListeningSessions.FindAsync(s => s.CreatedAt >= cutoff);
        var total = sessions.Count();
        var paginated = sessions.OrderBy(s => s.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(s => new EtlListeningSessionDto
        {
            SessionHash = DeidentificationHelper.HashId(s.Id),
            PatientHash = DeidentificationHelper.HashId(s.PatientId),
            TherapyType = DeidentificationHelper.MapTherapyType(s.TherapyType),
            Format = s.Format,
            MoodBefore = s.MoodBefore,
            MoodAfter = s.MoodAfter,
            DurationMinutes = s.DurationMinutes,
            RiskLevel = DeidentificationHelper.MapRiskLevel(s.RiskLevel),
            HomeworkCompleted = s.HomeworkCompleted,
            SessionQuarter = DeidentificationHelper.GeneralizeQuarter(s.SessionDate),
            CreatedAt = s.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 10. Visits
    public async Task<EtlResponse<List<EtlVisitDto>>> GetVisitsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var visits = await _unitOfWork.Visits.FindAsync(v => v.CreatedAt >= cutoff);
        var total = visits.Count();
        var paginated = visits.OrderBy(v => v.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(v => new EtlVisitDto
        {
            VisitHash = DeidentificationHelper.HashId(v.Id),
            PatientHash = DeidentificationHelper.HashId(v.PatientId),
            VisitType = DeidentificationHelper.MapVisitType(v.VisitType),
            VisitLocation = DeidentificationHelper.MapVisitLocation(v.VisitLocation),
            VisitQuarter = DeidentificationHelper.GeneralizeQuarter(v.VisitDate),
            CreatedAt = v.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }

    // 11. Treatment Plan Goals
    public async Task<EtlResponse<List<EtlTreatmentPlanGoalDto>>> GetTreatmentPlanGoalsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;

        // لاحظ أننا نستخدم الدالة الجديدة FindFullTreesAsync لتجنب البطء
        var plans = await _unitOfWork.TreatmentPlans.FindFullTreesAsync(p => p.CreatedAt >= cutoff);
        var allGoals = new List<EtlTreatmentPlanGoalDto>();

        foreach (var plan in plans)
        {
            if (plan.Problems == null) continue;
            foreach (var problem in plan.Problems)
            {
                foreach (var goal in problem.Goals)
                {
                    var targetValue = goal.TargetValue;
                    var baseline = goal.Baseline;
                    var currentValue = goal.CurrentValue;

                    int? progressPct = null;
                    if (targetValue.HasValue && baseline.HasValue)
                    {
                        var range = targetValue.Value - baseline.Value;
                        if (range != 0 && currentValue.HasValue)
                        {
                            var progress = ((currentValue.Value - baseline.Value) / range) * 100;
                            progressPct = (int)Math.Max(0, Math.Min(100, progress));
                        }
                    }

                    allGoals.Add(new EtlTreatmentPlanGoalDto
                    {
                        PlanHash = DeidentificationHelper.HashId(plan.Id),
                        GoalHash = DeidentificationHelper.HashId(goal.Id),
                        PatientHash = DeidentificationHelper.HashId(plan.PatientId),
                        PlanStatus = DeidentificationHelper.MapPlanStatus(plan.Status),
                        PlanDurationDays = DeidentificationHelper.CalculateDurationDays(plan.StartDate, plan.EndDate),
                        ProblemCategory = DeidentificationHelper.MapProblemCategory(problem.Text),
                        GoalStatus = goal.Status,
                        GoalBaseline = goal.Baseline,
                        GoalTarget = goal.TargetValue,
                        GoalCurrent = goal.CurrentValue,
                        GoalProgressPercentage = progressPct,
                        AchievedAt = goal.AchievedAt,
                        CreatedAt = plan.CreatedAt
                    });
                }
            }
        }

        var total = allGoals.Count;
        var paginated = allGoals.OrderBy(g => g.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Success(paginated, page, pageSize, total);
    }

    // 12. Vital Signs
    public async Task<EtlResponse<List<EtlVitalSignDto>>> GetVitalSignsAsync(DateTime? updatedSince, int page, int pageSize)
    {
        var cutoff = updatedSince ?? DateTime.MinValue;
        var vitals = await _unitOfWork.VitalSigns.FindAsync(v => v.CreatedAt >= cutoff);
        var total = vitals.Count();
        var paginated = vitals.OrderBy(v => v.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = paginated.Select(v => new EtlVitalSignDto
        {
            VitalHash = DeidentificationHelper.HashId(v.Id),
            PatientHash = DeidentificationHelper.HashId(v.PatientId),
            VitalType = DeidentificationHelper.MapVitalType(v.VitalType),
            Value1 = v.Value1,
            Value2 = v.Value2,
            Status = v.Status,
            MeasurementQuarter = DeidentificationHelper.GeneralizeQuarter(v.MeasurementDate),
            CreatedAt = v.CreatedAt
        }).ToList();
        return Success(dtos, page, pageSize, total);
    }
}