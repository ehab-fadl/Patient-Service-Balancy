using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.ChronicPsychDisease;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class ChronicPsychDiseaseService : IChronicPsychDiseaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChronicPsychDiseaseFactory _factory;
    private readonly ILogger<ChronicPsychDiseaseService> _logger;

    public ChronicPsychDiseaseService(
        IUnitOfWork unitOfWork,
        IChronicPsychDiseaseFactory factory,
        ILogger<ChronicPsychDiseaseService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ChronicPsychDiseaseResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<ChronicPsychDiseaseResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<ChronicPsychDiseaseResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var diseases = await _unitOfWork.ChronicPsychDiseases.GetByPatientIdAsync(patientId);
        var result = diseases.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<ChronicPsychDiseaseResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<ChronicPsychDiseaseResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var disease = await _unitOfWork.ChronicPsychDiseases.GetByIdAndDoctorAsync(id, doctorId);
        if (disease is null) return ServiceResult<ChronicPsychDiseaseResponseDto>.NotFoundResult("المرض غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<ChronicPsychDiseaseResponseDto>.SuccessResult(MapToResponse(disease));
    }

    public async Task<ServiceResult<ChronicPsychDiseaseResponseDto>> CreateAsync(ChronicPsychDiseaseCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<ChronicPsychDiseaseResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<ChronicPsychDiseaseResponseDto>.FailureResult("غير مصرح لك بإضافة مرض لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.ChronicPsychDiseases.AddAsync(entity);

            _logger.LogInformation("Chronic psych disease created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<ChronicPsychDiseaseResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة المرض النفسي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chronic psych disease for patient {PatientId}", dto.PatientId);
            return ServiceResult<ChronicPsychDiseaseResponseDto>.FailureResult("حدث خطأ أثناء إضافة المرض", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<ChronicPsychDiseaseResponseDto>> UpdateAsync(ChronicPsychDiseaseUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ChronicPsychDiseases.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<ChronicPsychDiseaseResponseDto>.NotFoundResult("المرض غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.ChronicPsychDiseases.UpdateAsync(existing);

            _logger.LogInformation("Chronic psych disease updated. Id={Id}", dto.Id);
            return ServiceResult<ChronicPsychDiseaseResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث المرض النفسي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chronic psych disease {Id}", dto.Id);
            return ServiceResult<ChronicPsychDiseaseResponseDto>.FailureResult("حدث خطأ أثناء تحديث المرض", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ChronicPsychDiseases.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("المرض غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.ChronicPsychDiseases.DeleteAsync(existing);

            _logger.LogInformation("Chronic psych disease deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف المرض النفسي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chronic psych disease {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء حذف المرض", new List<string> { ex.Message });
        }
    }

    private static ChronicPsychDiseaseResponseDto MapToResponse(ChronicPsychDisease entity)
    {
        return new ChronicPsychDiseaseResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Name = entity.Name,
            DiagnosisDate = entity.DiagnosisDate,
            Hospitalization = entity.Hospitalization,
            Medications = entity.Medications,
            Severity = entity.Severity,
            Response = entity.Response,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}