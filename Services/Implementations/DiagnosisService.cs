using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Diagnosis;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class DiagnosisService : IDiagnosisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiagnosisFactory _factory;
    private readonly ILogger<DiagnosisService> _logger;

    public DiagnosisService(
        IUnitOfWork unitOfWork,
        IDiagnosisFactory factory,
        ILogger<DiagnosisService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<DiagnosisResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<DiagnosisResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<DiagnosisResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var diagnoses = await _unitOfWork.Diagnoses.GetByPatientIdAsync(patientId);
        var result = diagnoses.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<DiagnosisResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<DiagnosisResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var diagnosis = await _unitOfWork.Diagnoses.GetByIdAndDoctorAsync(id, doctorId);
        if (diagnosis is null) return ServiceResult<DiagnosisResponseDto>.NotFoundResult("التشخيص غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<DiagnosisResponseDto>.SuccessResult(MapToResponse(diagnosis));
    }

    public async Task<ServiceResult<DiagnosisResponseDto>> CreateAsync(DiagnosisCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<DiagnosisResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<DiagnosisResponseDto>.FailureResult("غير مصرح لك بإضافة تشخيص لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.Diagnoses.AddAsync(entity);

            _logger.LogInformation("Diagnosis created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<DiagnosisResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة التشخيص بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating diagnosis for patient {PatientId}", dto.PatientId);
            return ServiceResult<DiagnosisResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<DiagnosisResponseDto>> UpdateAsync(DiagnosisUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Diagnoses.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<DiagnosisResponseDto>.NotFoundResult("التشخيص غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.Diagnoses.UpdateAsync(existing);

            _logger.LogInformation("Diagnosis updated. Id={Id}", dto.Id);
            return ServiceResult<DiagnosisResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث التشخيص بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating diagnosis {Id}", dto.Id);
            return ServiceResult<DiagnosisResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Diagnoses.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("التشخيص غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.Diagnoses.DeleteAsync(existing);

            _logger.LogInformation("Diagnosis deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف التشخيص بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting diagnosis {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static DiagnosisResponseDto MapToResponse(Diagnosis entity)
    {
        return new DiagnosisResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            MainType = entity.MainType,
            SubType = entity.SubType,
            DiagnosisDate = entity.DiagnosisDate,
            Severity = entity.Severity,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}