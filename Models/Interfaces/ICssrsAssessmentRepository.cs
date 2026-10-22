using PatientService.Models.Entities;

namespace PatientService.Models.Interfaces;

public interface ICssrsAssessmentRepository : IGenericRepository<CssrsAssessment>
{
    Task<IEnumerable<CssrsAssessment>> GetByPatientIdAsync(Guid patientId);
    Task<CssrsAssessment?> GetByIdAndDoctorAsync(Guid id, Guid doctorId);
}