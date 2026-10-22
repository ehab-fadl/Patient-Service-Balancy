using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.VitalSign;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class VitalSignService : IVitalSignService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVitalSignFactory _factory;
    private readonly ILogger<VitalSignService> _logger;

    public VitalSignService(
        IUnitOfWork unitOfWork,
        IVitalSignFactory factory,
        ILogger<VitalSignService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<VitalSignResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<VitalSignResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<VitalSignResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var vitals = await _unitOfWork.VitalSigns.GetByPatientIdAsync(patientId);
        var result = vitals.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<VitalSignResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<VitalSignResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var vital = await _unitOfWork.VitalSigns.GetByIdAndDoctorAsync(id, doctorId);
        if (vital is null) return ServiceResult<VitalSignResponseDto>.NotFoundResult("القراءة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<VitalSignResponseDto>.SuccessResult(MapToResponse(vital));
    }

    public async Task<ServiceResult<VitalSignResponseDto>> CreateAsync(VitalSignCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<VitalSignResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<VitalSignResponseDto>.FailureResult("غير مصرح لك بإضافة قراءة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.VitalSigns.AddAsync(entity);

            _logger.LogInformation("Vital sign created. Id={Id}, Type={Type}", entity.Id, entity.VitalType);
            return ServiceResult<VitalSignResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة القراءة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vital sign for patient {PatientId}", dto.PatientId);
            return ServiceResult<VitalSignResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<VitalSignResponseDto>> UpdateAsync(VitalSignUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.VitalSigns.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<VitalSignResponseDto>.NotFoundResult("القراءة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.VitalSigns.UpdateAsync(existing);

            _logger.LogInformation("Vital sign updated. Id={Id}", dto.Id);
            return ServiceResult<VitalSignResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث القراءة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vital sign {Id}", dto.Id);
            return ServiceResult<VitalSignResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.VitalSigns.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("القراءة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.VitalSigns.DeleteAsync(existing);

            _logger.LogInformation("Vital sign deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف القراءة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vital sign {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static VitalSignResponseDto MapToResponse(VitalSign entity)
    {
        return new VitalSignResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            VitalType = entity.VitalType,
            MeasurementDate = entity.MeasurementDate,
            Value1 = entity.Value1,
            Value2 = entity.Value2,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}