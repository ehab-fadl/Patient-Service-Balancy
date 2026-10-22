using PatientService.Models.Entities;
using PatientService.Services.DTOs.PatientPersonalInfo;

namespace PatientService.Services.Factories;

public class PatientPersonalInfoFactory : IPatientPersonalInfoFactory
{
    public PatientPersonalInfo CreateFrom(PatientPersonalInfoCreateDto dto, Guid doctorId)
    {
        return new PatientPersonalInfo
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim().ToLowerInvariant(),
            Address = dto.Address.Trim(),
            City = dto.City?.Trim(),
            MaritalStatus = dto.MaritalStatus?.Trim(),
            DoctorId = doctorId
        };
    }

    public void UpdateFrom(PatientPersonalInfo entity, PatientPersonalInfoUpdateDto dto)
    {
        entity.FirstName = dto.FirstName.Trim();
        entity.LastName = dto.LastName.Trim();
        entity.DateOfBirth = dto.DateOfBirth;
        entity.Gender = dto.Gender.Trim();
        entity.Phone = dto.Phone?.Trim();
        entity.Email = dto.Email?.Trim().ToLowerInvariant();
        entity.Address = dto.Address.Trim();
        entity.City = dto.City?.Trim();
        entity.MaritalStatus = dto.MaritalStatus?.Trim();
        
    }
}