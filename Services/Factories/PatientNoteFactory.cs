using PatientService.Models.Entities;
using PatientService.Services.DTOs.PatientNote;

namespace PatientService.Services.Factories;

public class PatientNoteFactory : IPatientNoteFactory
{
    public PatientNote CreateFrom(PatientNoteCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new PatientNote
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Title = dto.Title.Trim(),
            NoteDate = dto.NoteDate.Date,
            NoteType = dto.NoteType.Trim(),
            Category = dto.Category?.Trim(),
            Author = dto.Author.Trim(),
            AuthorRole = dto.AuthorRole.Trim(),
            Priority = dto.Priority?.Trim(),
            RiskLevel = dto.RiskLevel?.Trim(),
            MoodAtNote = dto.MoodAtNote,
            Visibility = dto.Visibility.Trim(),
            Symptoms = string.IsNullOrWhiteSpace(dto.Symptoms) ? null : dto.Symptoms.Trim(),
            LinkType = dto.LinkType?.Trim(),
            LinkItemId = dto.LinkItemId,
            Content = dto.Content.Trim(),
            NextPlan = string.IsNullOrWhiteSpace(dto.NextPlan) ? null : dto.NextPlan.Trim()
        };
    }

    public void UpdateFrom(PatientNote entity, PatientNoteUpdateDto dto)
    {
        entity.Title = dto.Title.Trim();
        entity.NoteDate = dto.NoteDate.Date;
        entity.NoteType = dto.NoteType.Trim();
        entity.Category = dto.Category?.Trim();
        entity.Author = dto.Author.Trim();
        entity.AuthorRole = dto.AuthorRole.Trim();
        entity.Priority = dto.Priority?.Trim();
        entity.RiskLevel = dto.RiskLevel?.Trim();
        entity.MoodAtNote = dto.MoodAtNote;
        entity.Visibility = dto.Visibility.Trim();
        entity.Symptoms = string.IsNullOrWhiteSpace(dto.Symptoms) ? null : dto.Symptoms.Trim();
        entity.LinkType = dto.LinkType?.Trim();
        entity.LinkItemId = dto.LinkItemId;
        entity.Content = dto.Content.Trim();
        entity.NextPlan = string.IsNullOrWhiteSpace(dto.NextPlan) ? null : dto.NextPlan.Trim();
    }
}