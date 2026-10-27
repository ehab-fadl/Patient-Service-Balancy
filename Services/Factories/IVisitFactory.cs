using PatientService.Models.Entities;
using PatientService.Services.DTOs.Visit;

namespace PatientService.Services.Factories;

public interface IVisitFactory
{
    Visit CreateFrom(VisitCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(Visit entity, VisitUpdateDto dto);
}