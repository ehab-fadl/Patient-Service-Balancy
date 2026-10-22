using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.SdsAssessment;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class SdsAssessmentService : ISdsAssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISdsAssessmentFactory _factory;
    private readonly ILogger<SdsAssessmentService> _logger;

    public SdsAssessmentService(
        IUnitOfWork unitOfWork,
        ISdsAssessmentFactory factory,
        ILogger<SdsAssessmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<SdsAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<SdsAssessmentResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<SdsAssessmentResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var assessments = await _unitOfWork.SdsAssessments.GetByPatientIdAsync(patientId);
        var result = assessments.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<SdsAssessmentResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<SdsAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var assessment = await _unitOfWork.SdsAssessments.GetByIdAndDoctorAsync(id, doctorId);
        if (assessment is null) return ServiceResult<SdsAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<SdsAssessmentResponseDto>.SuccessResult(MapToResponse(assessment));
    }

    public async Task<ServiceResult<SdsAssessmentResponseDto>> CreateAsync(SdsAssessmentCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<SdsAssessmentResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<SdsAssessmentResponseDto>.FailureResult("غير مصرح لك بإضافة تقييم لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.SdsAssessments.AddAsync(entity);

            _logger.LogInformation("SDS assessment created. Id={Id}, PatientId={PatientId}, Score={Score}", entity.Id, entity.PatientId, entity.TotalScore);
            return ServiceResult<SdsAssessmentResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ نتيجة التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SDS assessment for patient {PatientId}", dto.PatientId);
            return ServiceResult<SdsAssessmentResponseDto>.FailureResult("حدث خطأ أثناء الحفظ", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<SdsAssessmentResponseDto>> UpdateAsync(SdsAssessmentUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SdsAssessments.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<SdsAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.SdsAssessments.UpdateAsync(existing);

            _logger.LogInformation("SDS assessment updated. Id={Id}", dto.Id);
            return ServiceResult<SdsAssessmentResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SDS assessment {Id}", dto.Id);
            return ServiceResult<SdsAssessmentResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SdsAssessments.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.SdsAssessments.DeleteAsync(existing);

            _logger.LogInformation("SDS assessment deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SDS assessment {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static SdsAssessmentResponseDto MapToResponse(SdsAssessment entity)
    {
        return new SdsAssessmentResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            TotalScore = entity.TotalScore,
            Severity = entity.Severity,
            AnswersJson = entity.AnswersJson,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}