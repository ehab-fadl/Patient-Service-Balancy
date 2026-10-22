using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Gad7Assessment;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class Gad7AssessmentService : IGad7AssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGad7AssessmentFactory _factory;
    private readonly ILogger<Gad7AssessmentService> _logger;

    public Gad7AssessmentService(
        IUnitOfWork unitOfWork,
        IGad7AssessmentFactory factory,
        ILogger<Gad7AssessmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<Gad7AssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<Gad7AssessmentResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<Gad7AssessmentResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var assessments = await _unitOfWork.Gad7Assessments.GetByPatientIdAsync(patientId);
        var result = assessments.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<Gad7AssessmentResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<Gad7AssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var assessment = await _unitOfWork.Gad7Assessments.GetByIdAndDoctorAsync(id, doctorId);
        if (assessment is null) return ServiceResult<Gad7AssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<Gad7AssessmentResponseDto>.SuccessResult(MapToResponse(assessment));
    }

    public async Task<ServiceResult<Gad7AssessmentResponseDto>> CreateAsync(Gad7AssessmentCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<Gad7AssessmentResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<Gad7AssessmentResponseDto>.FailureResult("غير مصرح لك بإضافة تقييم لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.Gad7Assessments.AddAsync(entity);

            _logger.LogInformation("GAD-7 assessment created. Id={Id}, PatientId={PatientId}, Score={Score}", entity.Id, entity.PatientId, entity.TotalScore);
            return ServiceResult<Gad7AssessmentResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ نتيجة التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating GAD-7 assessment for patient {PatientId}", dto.PatientId);
            return ServiceResult<Gad7AssessmentResponseDto>.FailureResult("حدث خطأ أثناء الحفظ", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<Gad7AssessmentResponseDto>> UpdateAsync(Gad7AssessmentUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Gad7Assessments.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<Gad7AssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.Gad7Assessments.UpdateAsync(existing);

            _logger.LogInformation("GAD-7 assessment updated. Id={Id}", dto.Id);
            return ServiceResult<Gad7AssessmentResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating GAD-7 assessment {Id}", dto.Id);
            return ServiceResult<Gad7AssessmentResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Gad7Assessments.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.Gad7Assessments.DeleteAsync(existing);

            _logger.LogInformation("GAD-7 assessment deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting GAD-7 assessment {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static Gad7AssessmentResponseDto MapToResponse(Gad7Assessment entity)
    {
        return new Gad7AssessmentResponseDto
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