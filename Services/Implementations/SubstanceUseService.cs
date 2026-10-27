using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.SubstanceUse;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class SubstanceUseService : ISubstanceUseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubstanceUseFactory _factory;
    private readonly ILogger<SubstanceUseService> _logger;

    public SubstanceUseService(
        IUnitOfWork unitOfWork,
        ISubstanceUseFactory factory,
        ILogger<SubstanceUseService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<SubstanceUseResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<SubstanceUseResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<SubstanceUseResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var substances = await _unitOfWork.SubstanceUses.GetByPatientIdAsync(patientId);
        var result = substances.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<SubstanceUseResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<SubstanceUseResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var substance = await _unitOfWork.SubstanceUses.GetByIdAndDoctorAsync(id, doctorId);
        if (substance is null) return ServiceResult<SubstanceUseResponseDto>.NotFoundResult("المادة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<SubstanceUseResponseDto>.SuccessResult(MapToResponse(substance));
    }

    public async Task<ServiceResult<SubstanceUseResponseDto>> CreateAsync(SubstanceUseCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<SubstanceUseResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<SubstanceUseResponseDto>.FailureResult("غير مصرح لك بإضافة مادة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.SubstanceUses.AddAsync(entity);

            _logger.LogInformation("Substance use created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<SubstanceUseResponseDto>.SuccessResult(MapToResponse(entity), "تمت إضافة المادة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating substance use for patient {PatientId}", dto.PatientId);
            return ServiceResult<SubstanceUseResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<SubstanceUseResponseDto>> UpdateAsync(SubstanceUseUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SubstanceUses.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<SubstanceUseResponseDto>.NotFoundResult("المادة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.SubstanceUses.UpdateAsync(existing);

            _logger.LogInformation("Substance use updated. Id={Id}", dto.Id);
            return ServiceResult<SubstanceUseResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث المادة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating substance use {Id}", dto.Id);
            return ServiceResult<SubstanceUseResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.SubstanceUses.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("المادة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.SubstanceUses.DeleteAsync(existing);

            _logger.LogInformation("Substance use deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف المادة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting substance use {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static SubstanceUseResponseDto MapToResponse(SubstanceUse entity)
    {
        return new SubstanceUseResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Name = entity.Name,
            Type = entity.Type,
            Route = entity.Route,
            Frequency = entity.Frequency,
            Amount = entity.Amount,
            DurationYears = entity.DurationYears,
            AgeOfOnset = entity.AgeOfOnset,
            Reason = entity.Reason,
            LastUseDate = entity.LastUseDate,
            CurrentStatus = entity.CurrentStatus,
            CravingLevel = entity.CravingLevel,
            Severity = entity.Severity,
            PhysicalDependence = entity.PhysicalDependence,
            PsychologicalDependence = entity.PsychologicalDependence,
            ImpactWork = entity.ImpactWork,
            ImpactRelationships = entity.ImpactRelationships,
            ImpactHealth = entity.ImpactHealth,
            ImpactLegal = entity.ImpactLegal,
            QuitAttempts = entity.QuitAttempts,
            LastQuitDate = entity.LastQuitDate,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}