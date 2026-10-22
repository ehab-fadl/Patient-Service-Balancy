using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface IMdqAssessmentRepository : IGenericRepository<MdqAssessment>
{
    Task<IEnumerable<MdqAssessment>> GetByPatientIdAsync(Guid patientId);
    Task<MdqAssessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}