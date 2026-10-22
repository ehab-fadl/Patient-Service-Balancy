using PatientService.Models.Entities;
using PatientService.Services.DTOs.VitalSign;

namespace PatientService.Services.Factories;

public class VitalSignFactory : IVitalSignFactory
{
    public VitalSign CreateFrom(VitalSignCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new VitalSign
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            VitalType = dto.VitalType.Trim(),
            MeasurementDate = dto.MeasurementDate,
            Value1 = dto.Value1,
            Value2 = dto.Value2,
            Status = dto.Status.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };
    }

    public void UpdateFrom(VitalSign entity, VitalSignUpdateDto dto)
    {
        entity.VitalType = dto.VitalType.Trim();
        entity.MeasurementDate = dto.MeasurementDate;
        entity.Value1 = dto.Value1;
        entity.Value2 = dto.Value2;
        entity.Status = dto.Status.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }
}