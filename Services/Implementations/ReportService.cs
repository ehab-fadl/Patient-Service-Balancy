using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Reports;
using PatientService.Services.Interfaces;
using System.Text.Json;

namespace PatientService.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IUnitOfWork unitOfWork, ILogger<ReportService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // 1. جلب البيانات الحالية للمريض
    public async Task<ServiceResult<PatientSnapshotDto>> GetPatientSnapshotAsync(Guid patientId, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(patientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<PatientSnapshotDto>.NotFoundResult("المريض غير موجود أو غير مصرح");

        // جلب التشخيص الأخير
        var diagnoses = await _unitOfWork.Diagnoses.GetByPatientIdAsync(patientId);
        var latestDiagnosis = diagnoses.OrderByDescending(d => d.DiagnosisDate).FirstOrDefault()?.MainType;

        // جلب الأدوية النشطة
        var medications = await _unitOfWork.Medications.GetByPatientIdAsync(patientId);
        var activeMeds = medications.Where(m => m.Status == "نشط").Select(m => $"{m.Name} {m.Dose}").ToList();

        var snapshot = new PatientSnapshotDto
        {
            PatientId = patient.Id,
            FullName = $"{patient.FirstName} {patient.LastName}",
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            CurrentDiagnosis = latestDiagnosis,
            CurrentMedications = activeMeds
        };

        return ServiceResult<PatientSnapshotDto>.SuccessResult(snapshot);
    }

    // 2. إنشاء التقرير مع لقطة بيانات
    public async Task<ServiceResult<ReportResponseDto>> CreateReportAsync(ReportCreateDto dto, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<ReportResponseDto>.NotFoundResult("المريض غير موجود أو غير مصرح");

        var diagnoses = await _unitOfWork.Diagnoses.GetByPatientIdAsync(dto.PatientId);
        var latestDiagnosis = diagnoses.OrderByDescending(d => d.DiagnosisDate).FirstOrDefault()?.MainType;

        // تحديد ما إذا كان التقرير قانونياً بناءً على النوع (من الـ Frontend)
        var legalTypes = new[] { "consent", "legalReport", "violenceRisk", "treatmentPlan", "discharge",
            "refusal", "capacity", "involuntary", "fitnessWork", "mandatoryReport",
            "roi", "antidepressantMonitoring", "ectPre", "secondOpinion", "npp" };
        var isLegal = legalTypes.Contains(dto.ReportType);

        var entity = new PatientReport
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            DoctorId = doctorId,
            ReportType = dto.ReportType,
            ReportTitle = dto.ReportTitle,
            IsLegal = isLegal,
            PatientNameSnapshot = $"{patient.FirstName} {patient.LastName}",
            PatientDobSnapshot = patient.DateOfBirth,
            PatientGenderSnapshot = patient.Gender,
            PatientDiagnosisSnapshot = latestDiagnosis,
            FormDataJson = JsonSerializer.Serialize(dto.FormData ?? new Dictionary<string, object>()),
            Notes = dto.Notes,
            Status = "Draft",
            DocumentDate = dto.DocumentDate,
            IsFavorite = false
        };

        await _unitOfWork.PatientReports.AddAsync(entity);
        _logger.LogInformation("Report created. Id={Id}, Type={Type}, IsLegal={IsLegal}", entity.Id, entity.ReportType, entity.IsLegal);

        return ServiceResult<ReportResponseDto>.SuccessResult(MapToResponse(entity), "تم حفظ التقرير بنجاح");
    }

    // 3. جلب التقارير
    public async Task<ServiceResult<IEnumerable<ReportResponseDto>>> GetReportsByPatientAsync(Guid patientId, Guid doctorId)
    {
        var reports = await _unitOfWork.PatientReports.GetByPatientIdAsync(patientId, doctorId);
        var result = reports.Select(MapToResponse).ToList();
        return ServiceResult<IEnumerable<ReportResponseDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<ReportResponseDto>> GetReportByIdAsync(Guid id, Guid doctorId)
    {
        var report = await _unitOfWork.PatientReports.GetByIdAndDoctorAsync(id, doctorId);
        if (report is null) return ServiceResult<ReportResponseDto>.NotFoundResult("التقرير غير موجود");
        return ServiceResult<ReportResponseDto>.SuccessResult(MapToResponse(report));
    }

    // 4. حذف تقرير
    public async Task<ServiceResult<bool>> DeleteReportAsync(Guid id, Guid doctorId)
    {
        var report = await _unitOfWork.PatientReports.GetByIdAndDoctorAsync(id, doctorId);
        if (report is null) return ServiceResult<bool>.NotFoundResult("التقرير غير موجود");

        await _unitOfWork.PatientReports.DeleteAsync(report);
        return ServiceResult<bool>.SuccessResult(true, "تم حذف التقرير بنجاح");
    }

    // 5. المفضلة
    public async Task<ServiceResult<bool>> ToggleFavoriteAsync(Guid id, Guid doctorId)
    {
        var report = await _unitOfWork.PatientReports.GetByIdAndDoctorAsync(id, doctorId);
        if (report is null) return ServiceResult<bool>.NotFoundResult("التقرير غير موجود");

        report.IsFavorite = !report.IsFavorite;
        await _unitOfWork.PatientReports.UpdateAsync(report);

        return ServiceResult<bool>.SuccessResult(report.IsFavorite, report.IsFavorite ? "أضيف للمفضلة" : "أزيل من المفضلة");
    }

    // 6. تحويل الكيان إلى DTO
    private static ReportResponseDto MapToResponse(PatientReport entity)
    {
        return new ReportResponseDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            ReportType = entity.ReportType,
            ReportTitle = entity.ReportTitle,
            IsLegal = entity.IsLegal,
            DocumentDate = entity.DocumentDate,
            Status = entity.Status,
            IsFavorite = entity.IsFavorite,
            PatientName = entity.PatientNameSnapshot,
            PatientDob = entity.PatientDobSnapshot,
            PatientGender = entity.PatientGenderSnapshot,
            PatientDiagnosis = entity.PatientDiagnosisSnapshot,
            FormDataJson = entity.FormDataJson,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}