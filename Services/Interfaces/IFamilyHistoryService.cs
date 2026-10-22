using PatientService.Services.Common;
using PatientService.Services.DTOs.FamilyHistory;

namespace PatientService.Services.Interfaces;

public interface IFamilyHistoryService
{
    Task<ServiceResult<IEnumerable<FamilyHistoryResponseDto>>> GetByPatientIdAsync(Guid patientId, Guid doctorId);
    Task<ServiceResult<FamilyHistoryResponseDto>> GetByIdAsync(Guid id, Guid doctorId);
    Task<ServiceResult<FamilyHistoryResponseDto>> CreateAsync(FamilyHistoryCreateDto dto, Guid doctorId);
    Task<ServiceResult<FamilyHistoryResponseDto>> UpdateAsync(FamilyHistoryUpdateDto dto, Guid doctorId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid doctorId);
}