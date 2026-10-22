using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Phq9Assessment;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class Phq9AssessmentService : IPhq9AssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhq9AssessmentFactory _factory;
    private readonly ILogger<Phq9AssessmentService> _logger;

    public Phq9AssessmentService(
        IUnitOfWork unitOfWork,
        IPhq9AssessmentFactory factory,
        ILogger<Phq9AssessmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<Phq9AssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<Phq9AssessmentResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<Phq9AssessmentResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var assessments = await _unitOfWork.Phq9Assessments.GetByPatientIdAsync(patientId);
        var result = assessments.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<Phq9AssessmentResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<Phq9AssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var assessment = await _unitOfWork.Phq9Assessments.GetByIdAndDoctorAsync(id, doctorId);
        if (assessment is null) return ServiceResult<Phq9AssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<Phq9AssessmentResponseDto>.SuccessResult(MapToResponse(assessment));
    }

    public async Task<ServiceResult<Phq9AssessmentResponseDto>> CreateAsync(Phq9AssessmentCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<Phq9AssessmentResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<Phq9AssessmentResponseDto>.FailureResult("غير مصرح لك بإضافة تقييم لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.Phq9Assessments.AddAsync(entity);

            _logger.LogInformation("PHQ-9 assessment created. Id={Id}, PatientId={PatientId}, Score={Score}", entity.Id, entity.PatientId, entity.TotalScore);
            return ServiceResult<Phq9AssessmentResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ نتيجة التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating PHQ-9 assessment for patient {PatientId}", dto.PatientId);
            return ServiceResult<Phq9AssessmentResponseDto>.FailureResult("حدث خطأ أثناء الحفظ", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<Phq9AssessmentResponseDto>> UpdateAsync(Phq9AssessmentUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Phq9Assessments.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<Phq9AssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.Phq9Assessments.UpdateAsync(existing);

            _logger.LogInformation("PHQ-9 assessment updated. Id={Id}", dto.Id);
            return ServiceResult<Phq9AssessmentResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating PHQ-9 assessment {Id}", dto.Id);
            return ServiceResult<Phq9AssessmentResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Phq9Assessments.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.Phq9Assessments.DeleteAsync(existing);

            _logger.LogInformation("PHQ-9 assessment deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting PHQ-9 assessment {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static Phq9AssessmentResponseDto MapToResponse(Phq9Assessment entity)
    {
        return new Phq9AssessmentResponseDto
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