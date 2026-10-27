using PatientService.Models.Entities;
using PatientService.Services.DTOs.ListeningSession;

namespace PatientService.Services.Factories;

public class ListeningSessionFactory : IListeningSessionFactory
{
    public ListeningSession CreateFrom(ListeningSessionCreateDto dto, Guid patientId, Guid doctorId)
    {
        return new ListeningSession
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Title = dto.Title.Trim(),
            SessionDate = dto.SessionDate.Date,
            SessionTime = dto.SessionTime,
            DurationMinutes = dto.DurationMinutes,
            SessionType = dto.SessionType?.Trim(),
            Status = dto.Status.Trim(),
            TherapyType = dto.TherapyType?.Trim(),
            Format = dto.Format?.Trim(),
            TherapistName = dto.TherapistName?.Trim(),
            LinkedTreatmentPlanId = dto.LinkedTreatmentPlanId,
            MoodBefore = dto.MoodBefore,
            MoodAfter = dto.MoodAfter,
            TopicsDiscussed = string.IsNullOrWhiteSpace(dto.TopicsDiscussed) ? null : dto.TopicsDiscussed.Trim(),
            TechniquesUsed = string.IsNullOrWhiteSpace(dto.TechniquesUsed) ? null : dto.TechniquesUsed.Trim(),
            RiskLevel = dto.RiskLevel?.Trim(),
            HomeworkCompleted = dto.HomeworkCompleted,
            HomeworkAssigned = string.IsNullOrWhiteSpace(dto.HomeworkAssigned) ? null : dto.HomeworkAssigned.Trim(),
            NextSessionPlan = string.IsNullOrWhiteSpace(dto.NextSessionPlan) ? null : dto.NextSessionPlan.Trim(),
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            ClinicalNotes = string.IsNullOrWhiteSpace(dto.ClinicalNotes) ? null : dto.ClinicalNotes.Trim()
        };
    }

    public void UpdateFrom(ListeningSession entity, ListeningSessionUpdateDto dto)
    {
        entity.Title = dto.Title.Trim();
        entity.SessionDate = dto.SessionDate.Date;
        entity.SessionTime = dto.SessionTime;
        entity.DurationMinutes = dto.DurationMinutes;
        entity.SessionType = dto.SessionType?.Trim();
        entity.Status = dto.Status.Trim();
        entity.TherapyType = dto.TherapyType?.Trim();
        entity.Format = dto.Format?.Trim();
        entity.TherapistName = dto.TherapistName?.Trim();
        entity.LinkedTreatmentPlanId = dto.LinkedTreatmentPlanId;
        entity.MoodBefore = dto.MoodBefore;
        entity.MoodAfter = dto.MoodAfter;
        entity.TopicsDiscussed = string.IsNullOrWhiteSpace(dto.TopicsDiscussed) ? null : dto.TopicsDiscussed.Trim();
        entity.TechniquesUsed = string.IsNullOrWhiteSpace(dto.TechniquesUsed) ? null : dto.TechniquesUsed.Trim();
        entity.RiskLevel = dto.RiskLevel?.Trim();
        entity.HomeworkCompleted = dto.HomeworkCompleted;
        entity.HomeworkAssigned = string.IsNullOrWhiteSpace(dto.HomeworkAssigned) ? null : dto.HomeworkAssigned.Trim();
        entity.NextSessionPlan = string.IsNullOrWhiteSpace(dto.NextSessionPlan) ? null : dto.NextSessionPlan.Trim();
        entity.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
        entity.ClinicalNotes = string.IsNullOrWhiteSpace(dto.ClinicalNotes) ? null : dto.ClinicalNotes.Trim();
    }
}