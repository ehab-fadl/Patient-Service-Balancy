using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.CssrsAssessment;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class CssrsAssessmentService : ICssrsAssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICssrsAssessmentFactory _factory;
    private readonly ILogger<CssrsAssessmentService> _logger;

    public CssrsAssessmentService(
        IUnitOfWork unitOfWork,
        ICssrsAssessmentFactory factory,
        ILogger<CssrsAssessmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<CssrsAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<CssrsAssessmentResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<CssrsAssessmentResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var assessments = await _unitOfWork.CssrsAssessments.GetByPatientIdAsync(patientId);
        var result = assessments.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<CssrsAssessmentResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<CssrsAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var assessment = await _unitOfWork.CssrsAssessments.GetByIdAndDoctorAsync(id, doctorId);
        if (assessment is null) return ServiceResult<CssrsAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<CssrsAssessmentResponseDto>.SuccessResult(MapToResponse(assessment));
    }

    public async Task<ServiceResult<CssrsAssessmentResponseDto>> CreateAsync(CssrsAssessmentCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<CssrsAssessmentResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<CssrsAssessmentResponseDto>.FailureResult("غير مصرح لك بإضافة تقييم لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.CssrsAssessments.AddAsync(entity);

            // سجل تنبيه في الـ Log في حال كان حرجاً
            if (entity.IsCritical)
            {
                _logger.LogWarning("CRITICAL C-SSRS RISK DETECTED! PatientId: {PatientId}, Score: {Score}, DoctorId: {DoctorId}", entity.PatientId, entity.TotalScore, doctorId);
            }
            else
            {
                _logger.LogInformation("C-SSRS assessment created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            }

            return ServiceResult<CssrsAssessmentResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ تقييم الخطورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating C-SSRS assessment for patient {PatientId}", dto.PatientId);
            return ServiceResult<CssrsAssessmentResponseDto>.FailureResult("حدث خطأ أثناء الحفظ", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<CssrsAssessmentResponseDto>> UpdateAsync(CssrsAssessmentUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.CssrsAssessments.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<CssrsAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.CssrsAssessments.UpdateAsync(existing);

            _logger.LogInformation("C-SSRS assessment updated. Id={Id}", dto.Id);
            return ServiceResult<CssrsAssessmentResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating C-SSRS assessment {Id}", dto.Id);
            return ServiceResult<CssrsAssessmentResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.CssrsAssessments.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.CssrsAssessments.DeleteAsync(existing);

            _logger.LogInformation("C-SSRS assessment deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting C-SSRS assessment {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static CssrsAssessmentResponseDto MapToResponse(CssrsAssessment entity)
    {
        return new CssrsAssessmentResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            TotalScore = entity.TotalScore,
            Severity = entity.Severity,
            IsCritical = entity.IsCritical,
            AnswersJson = entity.AnswersJson,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}