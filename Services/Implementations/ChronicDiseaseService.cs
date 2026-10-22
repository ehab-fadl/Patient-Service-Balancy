using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.ChronicDisease;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class ChronicDiseaseService : IChronicDiseaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChronicDiseaseFactory _factory;
    private readonly ILogger<ChronicDiseaseService> _logger;

    public ChronicDiseaseService(
        IUnitOfWork unitOfWork,
        IChronicDiseaseFactory factory,
        ILogger<ChronicDiseaseService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ChronicDiseaseResponseDto>>> GetByPatientIdAsync(
        Guid patientId, Guid doctorId)
    {
        // التحقق من ملكية المريض
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null)
            return ServiceResult<IEnumerable<ChronicDiseaseResponseDto>>
                .NotFoundResult("المريض غير موجود");

        if (patient.DoctorId != doctorId)
            return ServiceResult<IEnumerable<ChronicDiseaseResponseDto>>
                .FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var diseases = await _unitOfWork.ChronicDiseases.GetByPatientIdAsync(patientId);

        var result = diseases.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<ChronicDiseaseResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<ChronicDiseaseResponseDto>> GetByIdAsync(
        Guid id, Guid doctorId)
    {
        var disease = await _unitOfWork.ChronicDiseases.GetByIdAndDoctorAsync(id, doctorId);

        if (disease is null)
            return ServiceResult<ChronicDiseaseResponseDto>
                .NotFoundResult("المرض غير موجود أو غير مصرح لك بالوصول إليه");

        return ServiceResult<ChronicDiseaseResponseDto>.SuccessResult(MapToResponse(disease));
    }

    public async Task<ServiceResult<ChronicDiseaseResponseDto>> CreateAsync(
        ChronicDiseaseCreateDto dto, Guid doctorId)
    {
        try
        {
            // 1) التحقق من ملكية المريض
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null)
                return ServiceResult<ChronicDiseaseResponseDto>.NotFoundResult("المريض غير موجود");

            if (patient.DoctorId != doctorId)
                return ServiceResult<ChronicDiseaseResponseDto>
                    .FailureResult("غير مصرح لك بإضافة مرض لهذا المريض");

            // 2) إنشاء المرض
            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.ChronicDiseases.AddAsync(entity);

            _logger.LogInformation(
                "Chronic disease created. Id={Id}, PatientId={PatientId}, DoctorId={DoctorId}",
                entity.Id, entity.PatientId, doctorId);

            return ServiceResult<ChronicDiseaseResponseDto>.SuccessResult(
                MapToResponse(entity),
                "تمت إضافة المرض المزمن بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chronic disease for patient {PatientId}", dto.PatientId);
            return ServiceResult<ChronicDiseaseResponseDto>.FailureResult(
                "حدث خطأ أثناء إضافة المرض",
                new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<ChronicDiseaseResponseDto>> UpdateAsync(
        ChronicDiseaseUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ChronicDiseases.GetByIdAndDoctorAsync(dto.Id, doctorId);

            if (existing is null)
                return ServiceResult<ChronicDiseaseResponseDto>
                    .NotFoundResult("المرض غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.ChronicDiseases.UpdateAsync(existing);

            _logger.LogInformation(
                "Chronic disease updated. Id={Id}, DoctorId={DoctorId}", dto.Id, doctorId);

            return ServiceResult<ChronicDiseaseResponseDto>.SuccessResult(
                MapToResponse(existing),
                "تم تحديث المرض المزمن بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chronic disease {Id}", dto.Id);
            return ServiceResult<ChronicDiseaseResponseDto>.FailureResult(
                "حدث خطأ أثناء تحديث المرض",
                new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.ChronicDiseases.GetByIdAndDoctorAsync(id, doctorId);

            if (existing is null)
                return ServiceResult<bool>
                    .NotFoundResult("المرض غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.ChronicDiseases.DeleteAsync(existing);

            _logger.LogInformation(
                "Chronic disease deleted. Id={Id}, DoctorId={DoctorId}", id, doctorId);

            return ServiceResult<bool>.SuccessResult(true, "تم حذف المرض المزمن بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chronic disease {Id}", id);
            return ServiceResult<bool>.FailureResult(
                "حدث خطأ أثناء حذف المرض",
                new List<string> { ex.Message });
        }
    }

    private static ChronicDiseaseResponseDto MapToResponse(ChronicDisease entity)
    {
        return new ChronicDiseaseResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Name = entity.Name,
            DiagnosisDate = entity.DiagnosisDate,
            Medications = entity.Medications,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}