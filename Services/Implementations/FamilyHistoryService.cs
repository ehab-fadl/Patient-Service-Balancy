using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.FamilyHistory;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class FamilyHistoryService : IFamilyHistoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFamilyHistoryFactory _factory;
    private readonly ILogger<FamilyHistoryService> _logger;

    public FamilyHistoryService(
        IUnitOfWork unitOfWork,
        IFamilyHistoryFactory factory,
        ILogger<FamilyHistoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<FamilyHistoryResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<FamilyHistoryResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<FamilyHistoryResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var histories = await _unitOfWork.FamilyHistories.GetByPatientIdAsync(patientId);
        var result = histories.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<FamilyHistoryResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<FamilyHistoryResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var history = await _unitOfWork.FamilyHistories.GetByIdAndDoctorAsync(id, doctorId);
        if (history is null) return ServiceResult<FamilyHistoryResponseDto>.NotFoundResult("السجل غير موجود أو غير مصرح لك بالوصول إليه");
        return ServiceResult<FamilyHistoryResponseDto>.SuccessResult(MapToResponse(history));
    }

    public async Task<ServiceResult<FamilyHistoryResponseDto>> CreateAsync(FamilyHistoryCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<FamilyHistoryResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<FamilyHistoryResponseDto>.FailureResult("غير مصرح لك بإضافة سجل لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.FamilyHistories.AddAsync(entity);

            _logger.LogInformation("Family history created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<FamilyHistoryResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة التاريخ العائلي بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family history for patient {PatientId}", dto.PatientId);
            return ServiceResult<FamilyHistoryResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<FamilyHistoryResponseDto>> UpdateAsync(FamilyHistoryUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.FamilyHistories.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<FamilyHistoryResponseDto>.NotFoundResult("السجل غير موجود أو غير مصرح لك بتعديله");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.FamilyHistories.UpdateAsync(existing);

            _logger.LogInformation("Family history updated. Id={Id}", dto.Id);
            return ServiceResult<FamilyHistoryResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث السجل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating family history {Id}", dto.Id);
            return ServiceResult<FamilyHistoryResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.FamilyHistories.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("السجل غير موجود أو غير مصرح لك بحذفه");

            await _unitOfWork.FamilyHistories.DeleteAsync(existing);

            _logger.LogInformation("Family history deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف السجل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting family history {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static FamilyHistoryResponseDto MapToResponse(FamilyHistory entity)
    {
        return new FamilyHistoryResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Relation = entity.Relation,
            RelativeName = entity.RelativeName,
            Age = entity.Age,
            Status = entity.Status,
            Severity = entity.Severity,
            ConditionType = entity.ConditionType,
            Conditions = entity.Conditions,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}