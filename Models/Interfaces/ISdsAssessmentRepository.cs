using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ISdsAssessmentRepository : IGenericRepository<SdsAssessment>
{
    Task<IEnumerable<SdsAssessment>> GetByPatientIdAsync(Guid patientId);
    Task<SdsAssessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}