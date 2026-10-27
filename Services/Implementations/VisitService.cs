using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Visit;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class VisitService : IVisitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVisitFactory _factory;
    private readonly ILogger<VisitService> _logger;

    public VisitService(
        IUnitOfWork unitOfWork,
        IVisitFactory factory,
        ILogger<VisitService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<VisitResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null) return ServiceResult<IEnumerable<VisitResponseDto>>.NotFoundResult("المريض غير موجود");
        if (patient.DoctorId != doctorId) return ServiceResult<IEnumerable<VisitResponseDto>>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var visits = await _unitOfWork.Visits.GetByPatientIdAsync(patientId);
        var result = visits.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<VisitResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<VisitResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var visit = await _unitOfWork.Visits.GetByIdAndDoctorAsync(id, doctorId);
        if (visit is null) return ServiceResult<VisitResponseDto>.NotFoundResult("الزيارة غير موجودة أو غير مصرح لك بالوصول إليها");
        return ServiceResult<VisitResponseDto>.SuccessResult(MapToResponse(visit));
    }

    public async Task<ServiceResult<VisitResponseDto>> CreateAsync(VisitCreateDto dto, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
            if (patient is null) return ServiceResult<VisitResponseDto>.NotFoundResult("المريض غير موجود");
            if (patient.DoctorId != doctorId) return ServiceResult<VisitResponseDto>.FailureResult("غير مصرح لك بإضافة زيارة لهذا المريض");

            var entity = _factory.CreateFrom(dto, dto.PatientId, doctorId);
            await _unitOfWork.Visits.AddAsync(entity);

            _logger.LogInformation("Visit created. Id={Id}, PatientId={PatientId}", entity.Id, entity.PatientId);
            return ServiceResult<VisitResponseDto>.SuccessResult(MapToResponse(entity), "تم تسجيل الزيارة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating visit for patient {PatientId}", dto.PatientId);
            return ServiceResult<VisitResponseDto>.FailureResult("حدث خطأ أثناء الإضافة", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<VisitResponseDto>> UpdateAsync(VisitUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Visits.GetByIdAndDoctorAsync(dto.Id, doctorId);
            if (existing is null) return ServiceResult<VisitResponseDto>.NotFoundResult("الزيارة غير موجودة أو غير مصرح لك بتعديلها");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.Visits.UpdateAsync(existing);

            _logger.LogInformation("Visit updated. Id={Id}", dto.Id);
            return ServiceResult<VisitResponseDto>.SuccessResult(MapToResponse(existing), "تم تحديث الزيارة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating visit {Id}", dto.Id);
            return ServiceResult<VisitResponseDto>.FailureResult("حدث خطأ أثناء التحديث", new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.Visits.GetByIdAndDoctorAsync(id, doctorId);
            if (existing is null) return ServiceResult<bool>.NotFoundResult("الزيارة غير موجودة أو غير مصرح لك بحذفها");

            await _unitOfWork.Visits.DeleteAsync(existing);

            _logger.LogInformation("Visit deleted. Id={Id}", id);
            return ServiceResult<bool>.SuccessResult(true, "تم حذف الزيارة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting visit {Id}", id);
            return ServiceResult<bool>.FailureResult("حدث خطأ أثناء الحذف", new List<string> { ex.Message });
        }
    }

    private static VisitResponseDto MapToResponse(Visit entity)
    {
        return new VisitResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Reason = entity.Reason,
            VisitDate = entity.VisitDate,
            VisitType = entity.VisitType,
            VisitLocation = entity.VisitLocation,
            Notes = entity.Notes,
            NextVisitDate = entity.NextVisitDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}