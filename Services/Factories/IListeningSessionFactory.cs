using PatientService.Models.Entities;
using PatientService.Services.DTOs.ListeningSession;

namespace PatientService.Services.Factories;

public interface IListeningSessionFactory
{
    ListeningSession CreateFrom(ListeningSessionCreateDto dto, Guid patientId, Guid doctorId);
    void UpdateFrom(ListeningSession entity, ListeningSessionUpdateDto dto);
}