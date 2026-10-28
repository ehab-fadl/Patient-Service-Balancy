using PatientService.Services.Common;
using PatientService.Services.DTOs.Reports;

namespace PatientService.Services.Interfaces;

public interface IReportService
{
    // 1. جلب البيانات الأولية للمريض لملء النموذج تلقائياً
    Task<ServiceResult<PatientSnapshotDto>> GetPatientSnapshotAsync(Guid patientId, Guid doctorId);

    // 2. إنشاء تقرير جديد
    Task<ServiceResult<ReportResponseDto>> CreateReportAsync(ReportCreateDto dto, Guid doctorId);

    // 3. جلب كل تقارير المريض
    Task<ServiceResult<IEnumerable<ReportResponseDto>>> GetReportsByPatientAsync(Guid patientId, Guid doctorId);

    // 4. جلب تقرير واحد
    Task<ServiceResult<ReportResponseDto>> GetReportByIdAsync(Guid id, Guid doctorId);

    // 5. حذف تقرير
    Task<ServiceResult<bool>> DeleteReportAsync(Guid id, Guid doctorId);

    // 6. تبديل المفضلة
    Task<ServiceResult<bool>> ToggleFavoriteAsync(Guid id, Guid doctorId);
}