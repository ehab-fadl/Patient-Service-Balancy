using Microsoft.AspNetCore.Http;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Documents;

namespace PatientService.Services.Interfaces;

public interface IDocumentService
{
    Task<ServiceResult<IEnumerable<DocumentResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<DocumentResponseDto>> GetByIdAsync(Guid id, Guid doctorId);

    // الرفع (يستقبل الملف + البيانات)
    Task<ServiceResult<DocumentResponseDto>> UploadDocumentAsync(
        Guid patientId,
        IFormFile file,
        string name, string type, string sourceType, DateTime documentDate,
        string? issuer, string? category, string? refNo, DateTime? expiryDate,
        bool isSigned, bool isCritical, string? notes, string? tags,
        Guid doctorId);

    Task<ServiceResult<DocumentResponseDto>> UpdateDocumentAsync(DocumentUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> ToggleFavoriteAsync(Guid id, Guid doctorId);
    Task<ServiceResult<bool>> DeleteDocumentAsync(Guid id, Guid doctorId);

    // التحميل
    Task<ServiceResult<byte[]>> DownloadFileAsync(Guid id, Guid doctorId);
}