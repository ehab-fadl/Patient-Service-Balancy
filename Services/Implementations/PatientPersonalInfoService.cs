using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.PatientCode;
using PatientService.Services.DTOs.PatientPersonalInfo;
using PatientService.Services.Factories;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class PatientPersonalInfoService : IPatientPersonalInfoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPatientPersonalInfoFactory _factory;
    private readonly IPatientCodeFactory _codeFactory;
    private readonly IPatientCodeGenerator _codeGenerator;
    private readonly IDepressionStoryFactory _storyFactory;
    private readonly ILogger<PatientPersonalInfoService> _logger;

    public PatientPersonalInfoService(
        IUnitOfWork unitOfWork,
        IPatientPersonalInfoFactory factory,
        IPatientCodeFactory codeFactory,
        IPatientCodeGenerator codeGenerator,
        IDepressionStoryFactory storyFactory,
        ILogger<PatientPersonalInfoService> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _codeFactory = codeFactory;
        _codeGenerator = codeGenerator;
        _storyFactory = storyFactory;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>> GetAllAsync(Guid doctorId)
    {
        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);

        var result = new List<PatientPersonalInfoResponseDto>();
        foreach (var patient in patients)
        {
            var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(patient.Id);
            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(patient.Id);
            result.Add(MapToResponse(patient, code, story));
        }

        return ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<PatientPersonalInfoResponseDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(id);

        if (patient is null)
            return ServiceResult<PatientPersonalInfoResponseDto>.NotFoundResult("المريض غير موجود");

        if (patient.DoctorId != doctorId)
            return ServiceResult<PatientPersonalInfoResponseDto>.FailureResult("غير مصرح لك بالوصول إلى هذا المريض");

        var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(patient.Id);
        var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(patient.Id);

        return ServiceResult<PatientPersonalInfoResponseDto>.SuccessResult(MapToResponse(patient, code, story));
    }

    public async Task<ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>> SearchAsync(Guid doctorId, string searchTerm)
    {
        var patients = await _unitOfWork.PatientPersonalInfos.SearchByDoctorIdAsync(doctorId, searchTerm);

        var result = new List<PatientPersonalInfoResponseDto>();
        foreach (var patient in patients)
        {
            var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(patient.Id);
            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(patient.Id);
            result.Add(MapToResponse(patient, code, story));
        }

        return ServiceResult<IEnumerable<PatientPersonalInfoResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<PatientPersonalInfoResponseDto>> CreateAsync(PatientPersonalInfoCreateDto dto, Guid doctorId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1) إنشاء المريض
            var patient = _factory.CreateFrom(dto, doctorId);
            await _unitOfWork.PatientPersonalInfos.AddAsync(patient);

            // 2) توليد كود فريد
            string code;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                code = _codeGenerator.Generate();
                attempts++;

                if (attempts > maxAttempts)
                    throw new InvalidOperationException("فشل توليد كود فريد للمريض بعد عدة محاولات");

            } while (await _unitOfWork.PatientCodes.CodeExistsAsync(code));

            var patientCode = _codeFactory.Create(patient.Id, code);
            await _unitOfWork.PatientCodes.AddAsync(patientCode);

            // 3) إنشاء قصة اكتئاب فارغة
            var depressionStory = _storyFactory.CreateEmptyFor(patient.Id);
            await _unitOfWork.DepressionStories.AddAsync(depressionStory);

            // 4) Commit
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Patient created. Id={PatientId}, Code={Code}, DoctorId={DoctorId}",
                patient.Id, code, doctorId);

            return ServiceResult<PatientPersonalInfoResponseDto>.SuccessResult(
                MapToResponse(patient, patientCode, depressionStory),
                "تم إنشاء المريض بنجاح مع كود فريد وقصة اكتئاب فارغة");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating patient for doctor {DoctorId}", doctorId);
            return ServiceResult<PatientPersonalInfoResponseDto>.FailureResult(
                "حدث خطأ أثناء إنشاء المريض",
                new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<PatientPersonalInfoResponseDto>> UpdateAsync(PatientPersonalInfoUpdateDto dto, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.Id);

            if (existing is null)
                return ServiceResult<PatientPersonalInfoResponseDto>.NotFoundResult("المريض غير موجود");

            if (existing.DoctorId != doctorId)
                return ServiceResult<PatientPersonalInfoResponseDto>.FailureResult("غير مصرح لك بتعديل هذا المريض");

            _factory.UpdateFrom(existing, dto);
            await _unitOfWork.PatientPersonalInfos.UpdateAsync(existing);

            var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(existing.Id);
            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(existing.Id);

            _logger.LogInformation("Patient {PatientId} updated by Doctor {DoctorId}", dto.Id, doctorId);

            return ServiceResult<PatientPersonalInfoResponseDto>.SuccessResult(
                MapToResponse(existing, code, story),
                "تم تحديث المريض بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient {PatientId}", dto.Id);
            return ServiceResult<PatientPersonalInfoResponseDto>.FailureResult(
                "حدث خطأ أثناء تحديث المريض",
                new List<string> { ex.Message });
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId)
    {
        try
        {
            var existing = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(id);

            if (existing is null)
                return ServiceResult<bool>.NotFoundResult("المريض غير موجود");

            if (existing.DoctorId != doctorId)
                return ServiceResult<bool>.FailureResult("غير مصرح لك بحذف هذا المريض");

            await _unitOfWork.PatientPersonalInfos.DeleteAsync(existing);

            // Soft Delete للكود
            var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(existing.Id);
            if (code is not null)
            {
                code.IsActive = false;
                await _unitOfWork.PatientCodes.DeleteAsync(code);
            }

            // Soft Delete لقصة الاكتئاب
            var story = await _unitOfWork.DepressionStories.GetByPatientIdAsync(existing.Id);
            if (story is not null)
            {
                await _unitOfWork.DepressionStories.DeleteAsync(story);
            }

            _logger.LogInformation("Patient {PatientId} deleted by Doctor {DoctorId}", id, doctorId);

            return ServiceResult<bool>.SuccessResult(true, "تم حذف المريض بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient {PatientId}", id);
            return ServiceResult<bool>.FailureResult(
                "حدث خطأ أثناء حذف المريض",
                new List<string> { ex.Message });
        }
    }

    private static PatientPersonalInfoResponseDto MapToResponse(
        PatientPersonalInfo entity,
        PatientCode? code,
        DepressionStory? story = null)
    {
        return new PatientPersonalInfoResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            DateOfBirth = entity.DateOfBirth,
            Age = CalculateAge(entity.DateOfBirth),
            Gender = entity.Gender,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            City = entity.City,
            MaritalStatus = entity.MaritalStatus,
            DoctorId = entity.DoctorId,

            PatientCode = code is not null ? new PatientCodeResponseDto
            {
                Id = code.Id,
                PatientId = code.PatientId,
                Code = code.Code,
                GeneratedAt = code.GeneratedAt,
                IsActive = code.IsActive
            } : null,

            HasDepressionStory = story is not null && !string.IsNullOrWhiteSpace(story.Story),
            DepressionStoryVersion = story?.StoryVersion ?? 0,

            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}