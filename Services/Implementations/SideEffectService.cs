using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.SideEffect;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class SideEffectService : ISideEffectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISideEffectFactory _factory;
    private readonly ILogger<SideEffectService> _logger;

    public SideEffectService(
        IUnitOfWork unitOfWork,
        ISideEffectFactory factory,
        ILogger<SideEffectService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<SideEffectResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<SideEffectResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<SideEffectResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var effects = await _unitOfWork.SideEffects.GetByPatientIdAsync(patientId);
        var result = effects.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<SideEffectResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<SideEffectResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var effect = await _unitOfWork.SideEffects.GetByIdAndDoctorAsync(id, doctorId);
        if (effect is null) return ServiceResult<SideEffectResponseDto>.NotFoundResult("العرض الجانبي غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<SideEffectResponseDto>.SuccessResult(MapToResponse(effect));
    }

    public async Task<ServiceResult<SideEffectResponseDto>> CreateAsync(SideEffectCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<SideEffectResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<SideEffectResponseDto>.FailureResult("غير مصرح لك بإضافة عرض جانبي لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.SideEffects.AddAsync(entity);

            _logger.LogInformation("Side effect created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<SideEffectResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة العرض الجانبي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating side effect for patient {PatientId}", dto.PatientId);
            return ServiceResult<SideEffectResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<SideEffectResponseDto>> UpdateAsync(SideEffectUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SideEffects.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<SideEffectResponseDto>.NotFoundResult("العرض الجانبي غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.SideEffects.UpdateAsync(existing);

            _logger.LogInformation("Side effect updated. Id={Id}", dto.Id);
            return ServiceResult<SideEffectResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث العرض الجانبي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating side effect {Id}", dto.Id);
            return ServiceResult<SideEffectResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SideEffects.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("العرض الجانبي غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.SideEffects.DeleteAsync(existing);

            _logger.LogInformation("Side effect deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف العرض الجانبي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting side effect {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static SideEffectResponseDto MapToResponse(SideEffect entity)
    {
        return new SideEffectResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            MedicationName = entity.MedicationName,
            Symptom = entity.Symptom,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}