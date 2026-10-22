using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Medication;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class MedicationService : IMedicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMedicationFactory _factory;
    private readonly ILogger<MedicationService> _logger;

    public MedicationService(
        IUnitOfWork unitOfWork,
        IMedicationFactory factory,
        ILogger<MedicationService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<MedicationResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<MedicationResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<MedicationResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var meds = await _unitOfWork.Medications.GetByPatientIdAsync(patientId);
        var result = meds.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<MedicationResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<MedicationResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var med = await _unitOfWork.Medications.GetByIdAndDoctorAsync(id, doctorId);
        if (med is null) return ServiceResult<MedicationResponseDto>.NotFoundResult("الدواء غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<MedicationResponseDto>.SuccessResult(MapToResponse(med));
    }

    public async Task<ServiceResult<MedicationResponseDto>> CreateAsync(MedicationCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<MedicationResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<MedicationResponseDto>.FailureResult("غير مصرح لك بإضافة دواء لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.Medications.AddAsync(entity);

            _logger.LogInformation("Medication created. Id={Id}, PatientId={PatientId}, Name={Name}", entity.Id, entity.PatientId, entity.Name);
            return ServiceResult<MedicationResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة الدواء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medication for patient {PatientId}", dto.PatientId);
            return ServiceResult<MedicationResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<MedicationResponseDto>> UpdateAsync(MedicationUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Medications.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<MedicationResponseDto>.NotFoundResult("الدواء غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.Medications.UpdateAsync(existing);

            _logger.LogInformation("Medication updated. Id={Id}", dto.Id);
            return ServiceResult<MedicationResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث الدواء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medication {Id}", dto.Id);
            return ServiceResult<MedicationResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Medications.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("الدواء غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.Medications.DeleteAsync(existing);

            _logger.LogInformation("Medication deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف الدواء بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medication {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static MedicationResponseDto MapToResponse(Medication entity)
    {
        return new MedicationResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Name = entity.Name,
            Dose = entity.Dose,
            Frequency = entity.Frequency,
            IntakePeriod = entity.IntakePeriod,
            Route = entity.Route,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}