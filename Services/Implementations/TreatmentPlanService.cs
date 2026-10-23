using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.TreatmentPlan;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class TreatmentPlanService : ITreatmentPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TreatmentPlanService> _logger;

    public TreatmentPlanService(IUnitOfWork unitOfWork, ILogger<TreatmentPlanService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────
    // Plans
    // ─────────────────────────────────────────────────────────
    public async Task<ServiceResult<IEnumerable<TreatmentPlanResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<TreatmentPlanResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<TreatmentPlanResponseDto>>.FailureResult("غير مصرح");

        var plans = await _unitOfWork.TreatmentPlans.GetFullTreeByPatientIdAsync(patientId);
        var dtos = plans.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<TreatmentPlanResponseDto>>.SuccessResult(dtos);
    }

    public async Task<ServiceResult<TreatmentPlanResponseDto>> CreatePlanAsync(TreatmentPlanCreateDto dto, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<TreatmentPlanResponseDto>.FailureResult("المريض غير موجود أو غير مصرح");

        var plan = new TreatmentPlan
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            DoctorId = doctorId,
            Name = dto.Name.Trim(),
            StartDate = dto.StartDate.Date,
            EndDate = dto.EndDate.Date,
            Status = dto.Status.Trim(),
            PlanDoctorName = dto.PlanDoctorName?.Trim(),
            Notes = dto.Notes?.Trim()
        };

        await _unitOfWork.TreatmentPlans.AddAsync(plan);
        _logger.LogInformation("Treatment plan created for patient {PatientId}", dto.PatientId);

        return ServiceResult<TreatmentPlanResponseDto>.SuccessResult(MapToResponse(plan), "تم إنشاء الخطة بنجاح");
    }

    public async Task<ServiceResult<TreatmentPlanResponseDto>> UpdatePlanAsync(TreatmentPlanUpdateDto dto, Guid doctorId)
    {
        var plan = await _unitOfWork.TreatmentPlans.GetFullTreeByIdAsync(dto.Id, doctorId);
        if (plan is null) return ServiceResult<TreatmentPlanResponseDto>.NotFoundResult("الخطة غير موجودة");

        plan.Name = dto.Name.Trim();
        plan.StartDate = dto.StartDate.Date;
        plan.EndDate = dto.EndDate.Date;
        plan.Status = dto.Status.Trim();
        plan.PlanDoctorName = dto.PlanDoctorName?.Trim();
        plan.Notes = dto.Notes?.Trim();

        await _unitOfWork.TreatmentPlans.UpdateAsync(plan);
        return ServiceResult<TreatmentPlanResponseDto>.SuccessResult(MapToResponse(plan), "تم تحديث الخطة بنجاح");
    }

    public async Task<ServiceResult<bool>> DeletePlanAsync(Guid id, Guid doctorId)
    {
        var plan = await _unitOfWork.TreatmentPlans.GetFullTreeByIdAsync(id, doctorId);
        if (plan is null) return ServiceResult<bool>.NotFoundResult("الخطة غير موجودة");

        await _unitOfWork.TreatmentPlans.DeleteAsync(plan);
        return ServiceResult<bool>.SuccessResult(true, "تم حذف الخطة بنجاح");
    }

    // ─────────────────────────────────────────────────────────
    // Problems
    // ─────────────────────────────────────────────────────────
    public async Task<ServiceResult<TreatmentProblemResponseDto>> AddProblemAsync(TreatmentProblemCreateDto dto, Guid doctorId)
    {
        var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(dto.TreatmentPlanId);
        if (plan is null || plan.DoctorId != doctorId)
            return ServiceResult<TreatmentProblemResponseDto>.NotFoundResult("الخطة غير موجودة");

        var problem = new TreatmentProblem
        {
            Id = Guid.NewGuid(),
            TreatmentPlanId = plan.Id,
            Text = dto.Text.Trim()
        };

      
        await _unitOfWork.TreatmentProblems.AddAsync(problem);

        return ServiceResult<TreatmentProblemResponseDto>.SuccessResult(
            new TreatmentProblemResponseDto { Id = problem.Id, Text = problem.Text },
            "تمت إضافة المشكلة بنجاح");
    }
    public async Task<ServiceResult<TreatmentProblemResponseDto>> UpdateProblemAsync(TreatmentProblemUpdateDto dto, Guid doctorId)
    {
        var problem = await _unitOfWork.TreatmentProblems.GetByIdAsync(dto.Id);
        if (problem is null) return ServiceResult<TreatmentProblemResponseDto>.NotFoundResult("المشكلة غير موجودة");

        problem.Text = dto.Text.Trim();
        await _unitOfWork.TreatmentProblems.UpdateAsync(problem);
        return ServiceResult<TreatmentProblemResponseDto>.SuccessResult(new TreatmentProblemResponseDto { Id = problem.Id, Text = problem.Text }, "تم التعديل");
    }

    public async Task<ServiceResult<bool>> DeleteProblemAsync(Guid id, Guid doctorId)
    {
        var problem = await _unitOfWork.TreatmentProblems.GetByIdAsync(id);
        if (problem is null) return ServiceResult<bool>.NotFoundResult("المشكلة غير موجودة");

        await _unitOfWork.TreatmentProblems.DeleteAsync(problem);
        return ServiceResult<bool>.SuccessResult(true, "تم الحذف");
    }

    // ─────────────────────────────────────────────────────────
    // Goals
    // ─────────────────────────────────────────────────────────
    public async Task<ServiceResult<TreatmentGoalResponseDto>> AddGoalAsync(TreatmentGoalCreateDto dto, Guid doctorId)
    {
        var problem = await _unitOfWork.TreatmentProblems.GetByIdAsync(dto.TreatmentProblemId);
        if (problem is null)
            return ServiceResult<TreatmentGoalResponseDto>.NotFoundResult("المشكلة غير موجودة");

        var goal = new TreatmentGoal
        {
            Id = Guid.NewGuid(),
            TreatmentProblemId = problem.Id,
            Text = dto.Text.Trim(),
            Status = "pending"
        };

      
        await _unitOfWork.TreatmentGoals.AddAsync(goal);

        return ServiceResult<TreatmentGoalResponseDto>.SuccessResult(
            MapToGoalResponse(goal),
            "تمت إضافة الهدف بنجاح");
    }

    public async Task<ServiceResult<TreatmentGoalResponseDto>> UpdateGoalConfigAsync(TreatmentGoalUpdateDto dto, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(dto.Id);
        if (goal is null) return ServiceResult<TreatmentGoalResponseDto>.NotFoundResult("الهدف غير موجود");

        goal.Text = dto.Text.Trim();
        goal.Unit = dto.Unit?.Trim();
        goal.Baseline = dto.Baseline;
        goal.TargetValue = dto.TargetValue;

        // إذا كانت القيمة الحالية فارغة، نضعها مساوية للقيمة الأولية
        if (goal.CurrentValue == null) goal.CurrentValue = dto.Baseline;

        await _unitOfWork.TreatmentGoals.UpdateAsync(goal);
        return ServiceResult<TreatmentGoalResponseDto>.SuccessResult(MapToGoalResponse(goal), "تم حفظ إعدادات الهدف");
    }

    public async Task<ServiceResult<TreatmentGoalResponseDto>> UpdateGoalProgressAsync(TreatmentGoalProgressDto dto, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(dto.GoalId);
        if (goal is null) return ServiceResult<TreatmentGoalResponseDto>.NotFoundResult("الهدف غير موجود");

        goal.CurrentValue = dto.Value;

        var history = new TreatmentGoalHistory
        {
            Id = Guid.NewGuid(),
            TreatmentGoalId = goal.Id,
            RecordedAt = DateTime.UtcNow,
            Value = dto.Value.ToString(),
            Note = dto.Note?.Trim()
        };

        await _unitOfWork.TreatmentGoalHistories.AddAsync(history);
        await _unitOfWork.TreatmentGoals.UpdateAsync(goal);

        // إعادة تحميل الهدف بالتاريخ
        goal.ProgressHistory.Add(history);

        return ServiceResult<TreatmentGoalResponseDto>.SuccessResult(MapToGoalResponse(goal), "تم تحديث التقدم بنجاح");
    }

    public async Task<ServiceResult<TreatmentGoalResponseDto>> AchieveGoalAsync(TreatmentGoalAchieveDto dto, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(dto.GoalId);
        if (goal is null) return ServiceResult<TreatmentGoalResponseDto>.NotFoundResult("الهدف غير موجود");

        goal.Status = "achieved";
        goal.AchievedAt = DateTime.UtcNow;
        goal.AchievedBy = "doctor";
        goal.AchievementNote = dto.Note?.Trim();
        if (goal.TargetValue.HasValue) goal.CurrentValue = goal.TargetValue;

        var history = new TreatmentGoalHistory
        {
            Id = Guid.NewGuid(),
            TreatmentGoalId = goal.Id,
            RecordedAt = DateTime.UtcNow,
            Value = "محقق ✅",
            Note = dto.Note?.Trim() ?? "تم التحقق بواسطة الطبيب"
        };

        await _unitOfWork.TreatmentGoalHistories.AddAsync(history);
        await _unitOfWork.TreatmentGoals.UpdateAsync(goal);

        return ServiceResult<TreatmentGoalResponseDto>.SuccessResult(MapToGoalResponse(goal), "تم تحقق الهدف بنجاح");
    }

    public async Task<ServiceResult<bool>> DeleteGoalAsync(Guid id, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(id);
        if (goal is null) return ServiceResult<bool>.NotFoundResult("الهدف غير موجود");

        await _unitOfWork.TreatmentGoals.DeleteAsync(goal);
        return ServiceResult<bool>.SuccessResult(true, "تم حذف الهدف");
    }

    // ─────────────────────────────────────────────────────────
    // Means
    // ─────────────────────────────────────────────────────────
    public async Task<ServiceResult<TreatmentMeanResponseDto>> AddMeanAsync(TreatmentMeanCreateDto dto, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(dto.TreatmentGoalId);
        if (goal is null) return ServiceResult<TreatmentMeanResponseDto>.NotFoundResult("الهدف غير موجود");

        var mean = new TreatmentMean { Id = Guid.NewGuid(), TreatmentGoalId = goal.Id, Text = dto.Text.Trim() };
        await _unitOfWork.TreatmentMeans.AddAsync(mean);

        return ServiceResult<TreatmentMeanResponseDto>.SuccessResult(new TreatmentMeanResponseDto { Id = mean.Id, Text = mean.Text }, "تمت إضافة الوسيلة");
    }

    public async Task<ServiceResult<TreatmentMeanResponseDto>> UpdateMeanAsync(TreatmentMeanUpdateDto dto, Guid doctorId)
    {
        var mean = await _unitOfWork.TreatmentMeans.GetByIdAsync(dto.Id);
        if (mean is null) return ServiceResult<TreatmentMeanResponseDto>.NotFoundResult("الوسيلة غير موجودة");

        mean.Text = dto.Text.Trim();
        await _unitOfWork.TreatmentMeans.UpdateAsync(mean);
        return ServiceResult<TreatmentMeanResponseDto>.SuccessResult(new TreatmentMeanResponseDto { Id = mean.Id, Text = mean.Text }, "تم التعديل");
    }

    public async Task<ServiceResult<bool>> DeleteMeanAsync(Guid id, Guid doctorId)
    {
        var mean = await _unitOfWork.TreatmentMeans.GetByIdAsync(id);
        if (mean is null) return ServiceResult<bool>.NotFoundResult("الوسيلة غير موجودة");

        await _unitOfWork.TreatmentMeans.DeleteAsync(mean);
        return ServiceResult<bool>.SuccessResult(true, "تم الحذف");
    }

    // ─────────────────────────────────────────────────────────
    // Mappers
    // ─────────────────────────────────────────────────────────
    private static TreatmentPlanResponseDto MapToResponse(TreatmentPlan plan)
    {
        return new TreatmentPlanResponseDto
        {
            Id = plan.Id,
            PatientId = plan.PatientId,
            Name = plan.Name,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            Status = plan.Status,
            PlanDoctorName = plan.PlanDoctorName,
            Notes = plan.Notes,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            Problems = plan.Problems.Select(pr => new TreatmentProblemResponseDto
            {
                Id = pr.Id,
                Text = pr.Text,
                Goals = pr.Goals.Select(g => MapToGoalResponse(g)).ToList()
            }).ToList()
        };
    }

    private static TreatmentGoalResponseDto MapToGoalResponse(TreatmentGoal goal)
    {
        return new TreatmentGoalResponseDto
        {
            Id = goal.Id,
            Text = goal.Text,
            Unit = goal.Unit,
            Baseline = goal.Baseline,
            TargetValue = goal.TargetValue,
            CurrentValue = goal.CurrentValue,
            Status = goal.Status,
            AchievedAt = goal.AchievedAt,
            AchievedBy = goal.AchievedBy,
            AchievementNote = goal.AchievementNote,
            PatientConfirmedByDoctor = goal.PatientConfirmedByDoctor,
            Means = goal.Means?.Select(m => new TreatmentMeanResponseDto { Id = m.Id, Text = m.Text }).ToList() ?? new(),
            ProgressHistory = goal.ProgressHistory?.Select(h => new TreatmentGoalHistoryResponseDto
            {
                Id = h.Id,
                RecordedAt = h.RecordedAt,
                Value = h.Value,
                Note = h.Note
            }).OrderBy(h => h.RecordedAt).ToList() ?? new()
        };
    }


    public async Task<ServiceResult<TreatmentGoalResponseDto>> UnachieveGoalAsync(TreatmentGoalUnachieveDto dto, Guid doctorId)
    {
        var goal = await _unitOfWork.TreatmentGoals.GetByIdAsync(dto.GoalId);
        if (goal is null)
            return ServiceResult<TreatmentGoalResponseDto>.NotFoundResult("الهدف غير موجود");

        // إعادة الهدف إلى قيد التنفيذ
        goal.Status = "pending";
        goal.AchievedAt = null;
        goal.AchievedBy = null;
        goal.AchievementNote = null;
        goal.PatientConfirmedByDoctor = false;

        var history = new TreatmentGoalHistory
        {
            Id = Guid.NewGuid(),
            TreatmentGoalId = goal.Id,
            RecordedAt = DateTime.UtcNow,
            Value = "تم إلغاء التحقق ⏪",
            Note = "تمت إعادة الهدف إلى قيد التنفيذ بواسطة الطبيب"
        };

        await _unitOfWork.TreatmentGoals.UpdateAsync(goal);
        await _unitOfWork.TreatmentGoalHistories.AddAsync(history);

        goal.ProgressHistory.Add(history);

        return ServiceResult<TreatmentGoalResponseDto>.SuccessResult(MapToGoalResponse(goal), "تم إلغاء تحقيق الهدف بنجاح");
    }
}