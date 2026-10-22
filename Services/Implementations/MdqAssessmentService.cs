using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.MdqAssessment;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class MdqAssessmentService : IMdqAssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMdqAssessmentFactory _factory;
    private readonly ILogger<MdqAssessmentService> _logger;

    public MdqAssessmentService(
        IUnitOfWork unitOfWork,
        IMdqAssessmentFactory factory,
        ILogger<MdqAssessmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<MdqAssessmentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<MdqAssessmentResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<MdqAssessmentResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var assessments = await _unitOfWork.MdqAssessments.GetByPatientIdAsync(patientId);
        var result = assessments.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<MdqAssessmentResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<MdqAssessmentResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var assessment = await _unitOfWork.MdqAssessments.GetByIdAndDoctorAsync(id, doctorId);
        if (assessment is null) return ServiceResult<MdqAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<MdqAssessmentResponseDto>.SuccessResult(MapToResponse(assessment));
    }

    public async Task<ServiceResult<MdqAssessmentResponseDto>> CreateAsync(MdqAssessmentCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<MdqAssessmentResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<MdqAssessmentResponseDto>.FailureResult("غير مصرح لك بإضافة تقييم لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.MdqAssessments.AddAsync(entity);

            _logger.LogInformation("MDQ assessment created. Id={Id}, PatientId={PatientId}, Score={Score}", entity.Id, entity.PatientId, entity.TotalScore);
            return ServiceResult<MdqAssessmentResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ نتيجة التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MDQ assessment for patient {PatientId}", dto.PatientId);
            return ServiceResult<MdqAssessmentResponseDto>.FailureResult("حدث خطأ أثناء الحفظ", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<MdqAssessmentResponseDto>> UpdateAsync(MdqAssessmentUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.MdqAssessments.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<MdqAssessmentResponseDto>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.MdqAssessments.UpdateAsync(existing);

            _logger.LogInformation("MDQ assessment updated. Id={Id}", dto.Id);
            return ServiceResult<MdqAssessmentResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MDQ assessment {Id}", dto.Id);
            return ServiceResult<MdqAssessmentResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.MdqAssessments.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التقييم غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.MdqAssessments.DeleteAsync(existing);

            _logger.LogInformation("MDQ assessment deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التقييم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MDQ assessment {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static MdqAssessmentResponseDto MapToResponse(MdqAssessment entity)
    {
        return new MdqAssessmentResponseDto
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