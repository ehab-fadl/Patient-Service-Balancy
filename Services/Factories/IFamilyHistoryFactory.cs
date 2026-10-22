using PatientService.Models.Entities;
using PatientService.Services.DTOs.FamilyHistory;

namespace PatientService.Services.Factories;

public interface IFamilyHistoryFactory
{
    FamilyHistory CreateFrom(FamilyHistoryCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(FamilyHistory entity, FamilyHistoryUpdateDto dto);
}