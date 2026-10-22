using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.DepressionStory;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class DepressionStoryService : IDepressionStoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDepressionStoryFactory _factory;
    private readonly ILogger<DepressionStoryService> _logger;

    public DepressionStoryService(
        IUnitOfWork unitOfWork,
        IDepressionStoryFactory factory,
        ILogger<DepressionStoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResult<DepressionStoryResponseDto>> GetByPatientIdAsync(Guid patientId, Guid doctorId)
    {
        // التحقق من أن المريض يخص هذا الطبيب
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);

        if (patient is null)
            return ServiceResult<DepressionStoryResponseDto>.NotFoundResult("المريض غير موجود");

        if (patient.DoctorId != doctorId)
            return ServiceResult<DepressionStoryResponseDto>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(patientId);

        if (story is null)
            return ServiceResult<DepressionStoryResponseDto>.NotFoundResult("لا توجد قصة اكتئاب لهذا المريض");

        return ServiceResult<DepressionStoryResponseDto>.SuccessResult(MapToResponse(story));
    }

    public async Task<ServiceResult<DepressionStoryResponseDto>> UpdateAsync(DepressionStoryUpdateDto dto, Guid doctorId)
    {
        try
        {
            // التحقق من أن المريض يخص هذا الطبيب
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);

            if (patient is null)
                return ServiceResult<DepressionStoryResponseDto>.NotFoundResult("المريض غير موجود");

            if (patient.DoctorId != doctorId)
                return ServiceResult<DepressionStoryResponseDto>.FailureResult("غير مصرح لك بتعديل قصة هذا المريض");

            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(dto.PatientId);

            if (story is null)
            {
                // إنشاء قصة جديدة إن لم تكن موجودة
                story = _factory.CreateEmptyFor(dto.PatientId);
                await _unitOfWork.DepressionStories.AddAsync(story);
            }

            _factory.UpdateFrom(story, dto.Story, dto.MarkAsReviewed);
            await _unitOfWork.DepressionStories.UpdateAsync(story);

            _logger.LogInformation(
                "Depression story updated for patient {PatientId} by doctor {DoctorId}. Version={Version}",
                dto.PatientId, doctorId, story.StoryVersion);

            return ServiceResult<DepressionStoryResponseDto>.SuccessResult(
                MapToResponse(story),
                "تم تحديث قصة الاكتئاب بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating depression story for patient {PatientId}", dto.PatientId);
            return ServiceResult<DepressionStoryResponseDto>.FailureResult(
                "حدث خطأ أثناء تحديث القصة",
                new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid patientId, Guid doctorId)
    {
        try
        {
            var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);

            if (patient is null)
                return ServiceResult<bool>.NotFoundResult("المريض غير موجود");

            if (patient.DoctorId != doctorId)
                return ServiceResult<bool>.FailureResult("غير مصرح لك بحذف قصة هذا المريض");

            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(patientId);

            if (story is null)
                return ServiceResult<bool>.NotFoundResult("لا توجد قصة اكتئاب لهذا المريض");

            await _unitOfWork.DepressionStories.DeleteAsync(story);

            _logger.LogInformation("Depression story deleted for patient {PatientId}", patientId);

            return ServiceResult<bool>.SuccessResult(true, "تم حذف قصة الاكتئاب بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting depression story for patient {PatientId}", patientId);
            return ServiceResult<bool>.FailureResult(
                "حدث خطأ أثناء حذف القصة",
                new List<string> { ex.Message });
        }
    }

    private static DepressionStoryResponseDto MapToResponse(DepressionStory entity)
    {
        return new DepressionStoryResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            Story = entity.Story,
            StoryVersion = entity.StoryVersion,
            LastReviewedAt = entity.LastReviewedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}