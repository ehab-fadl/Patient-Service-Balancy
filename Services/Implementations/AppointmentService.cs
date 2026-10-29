using Microsoft.Extensions.Logging;
using PatientService.Models.Entities;
using PatientService.Models.Interfaces;
using PatientService.Services.Common;
using PatientService.Services.DTOs.Appointments;
using PatientService.Services.Interfaces;

namespace PatientService.Services.Implementations;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IUnitOfWork unitOfWork, ILogger<AppointmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<AppointmentsSummaryDto>> GetAppointmentsAsync(Guid doctorId)
    {
        var appointments = (await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId)).ToList();
        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();

        var today = DateTime.UtcNow.Date;
        var weekEnd = today.AddDays(7);
        var monthEnd = today.AddDays(30);

        var dtos = appointments.Select(a => MapToDto(a, patients, codes)).ToList();

        var summary = new AppointmentsSummaryDto
        {
            Appointments = dtos,
            TodayCount = dtos.Count(a => a.ScheduledDate.Date == today),
            WeekCount = dtos.Count(a => a.ScheduledDate.Date >= today && a.ScheduledDate.Date <= weekEnd),
            MonthCount = dtos.Count(a => a.ScheduledDate.Date >= today && a.ScheduledDate.Date <= monthEnd),
            PendingCount = dtos.Count(a => a.Status == "Pending"),
            ConfirmedCount = dtos.Count(a => a.Status == "Confirmed"),
            CompletedCount = dtos.Count(a => a.Status == "Completed")
        };

        return ServiceResult<AppointmentsSummaryDto>.SuccessResult(summary);
    }

    public async Task<ServiceResult<AppointmentDto>> GetByIdAsync(Guid id, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(id, doctorId);
        if (appointment is null) return ServiceResult<AppointmentDto>.NotFoundResult("الموعد غير موجود");

        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();

        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, patients, codes));
    }

    public async Task<ServiceResult<AppointmentDto>> CreateAppointmentAsync(AppointmentCreateDto dto, Guid doctorId)
    {
        var patient = await _unitOfWork.PatientPersonalInfos.GetByIdAsync(dto.PatientId);
        if (patient is null || patient.DoctorId != doctorId)
            return ServiceResult<AppointmentDto>.NotFoundResult("المريض غير موجود أو غير مصرح");

        if (dto.ScheduledDate.Date < DateTime.UtcNow.Date)
            return ServiceResult<AppointmentDto>.FailureResult("لا يمكن حجز موعد في تاريخ سابق");

        // التحقق من تعارض المواعيد
        var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
            doctorId, dto.ScheduledDate, dto.ScheduledTime);
        if (hasConflict)
            return ServiceResult<AppointmentDto>.FailureResult("يوجد موعد آخر في نفس التاريخ والوقت");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            DoctorId = doctorId,
            ScheduledDate = dto.ScheduledDate.Date,
            ScheduledTime = dto.ScheduledTime,
            Reason = dto.Reason.Trim(),
            VisitType = dto.VisitType?.Trim() ?? "متابعة",
            VisitLocation = dto.VisitLocation?.Trim() ?? "عيادة",
            Notes = dto.Notes?.Trim(),
            Status = "Pending"
        };

        await _unitOfWork.Appointments.AddAsync(appointment);
        _logger.LogInformation("Appointment created. Id={Id}, PatientId={PatientId}", appointment.Id, dto.PatientId);

        var code = await _unitOfWork.PatientCodes.GetByPatientIdAsync(dto.PatientId);
        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, new[] { patient }, new[] { code! }), "تم حجز الموعد بنجاح");
    }

    public async Task<ServiceResult<AppointmentDto>> UpdateAppointmentAsync(AppointmentUpdateDto dto, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(dto.Id, doctorId);
        if (appointment is null) return ServiceResult<AppointmentDto>.NotFoundResult("الموعد غير موجود");

        if (appointment.Status == "Completed")
            return ServiceResult<AppointmentDto>.FailureResult("لا يمكن تعديل موعد مكتمل");

        // التحقق من التعارض (إذا تغير التاريخ/الوقت)
        if (appointment.ScheduledDate != dto.ScheduledDate.Date || appointment.ScheduledTime != dto.ScheduledTime)
        {
            var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                doctorId, dto.ScheduledDate, dto.ScheduledTime, dto.Id);
            if (hasConflict)
                return ServiceResult<AppointmentDto>.FailureResult("يوجد موعد آخر في نفس التاريخ والوقت");
        }

        appointment.ScheduledDate = dto.ScheduledDate.Date;
        appointment.ScheduledTime = dto.ScheduledTime;
        appointment.Reason = dto.Reason.Trim();
        appointment.VisitType = dto.VisitType?.Trim() ?? appointment.VisitType;
        appointment.VisitLocation = dto.VisitLocation?.Trim() ?? appointment.VisitLocation;
        appointment.Notes = dto.Notes?.Trim();

        await _unitOfWork.Appointments.UpdateAsync(appointment);

        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();
        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, patients, codes), "تم تحديث الموعد");
    }

    public async Task<ServiceResult<AppointmentDto>> ConfirmAppointmentAsync(Guid id, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(id, doctorId);
        if (appointment is null) return ServiceResult<AppointmentDto>.NotFoundResult("الموعد غير موجود");

        if (appointment.Status != "Pending")
            return ServiceResult<AppointmentDto>.FailureResult("لا يمكن تأكيد هذا الموعد");

        appointment.Status = "Confirmed";
        await _unitOfWork.Appointments.UpdateAsync(appointment);

        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();
        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, patients, codes), "تم تأكيد الموعد");
    }

    public async Task<ServiceResult<AppointmentDto>> CancelAppointmentAsync(AppointmentCancelDto dto, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(dto.Id, doctorId);
        if (appointment is null) return ServiceResult<AppointmentDto>.NotFoundResult("الموعد غير موجود");

        if (appointment.Status == "Completed")
            return ServiceResult<AppointmentDto>.FailureResult("لا يمكن إلغاء موعد مكتمل");

        appointment.Status = "Cancelled";
        appointment.CancellationReason = dto.CancellationReason.Trim();
        appointment.CancelledAt = DateTime.UtcNow;
        await _unitOfWork.Appointments.UpdateAsync(appointment);

        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();
        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, patients, codes), "تم إلغاء الموعد");
    }

    public async Task<ServiceResult<AppointmentDto>> MarkNoShowAsync(Guid id, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(id, doctorId);
        if (appointment is null) return ServiceResult<AppointmentDto>.NotFoundResult("الموعد غير موجود");

        if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            return ServiceResult<AppointmentDto>.FailureResult("لا يمكن تحديث حالة هذا الموعد");

        appointment.Status = "NoShow";
        await _unitOfWork.Appointments.UpdateAsync(appointment);

        var patients = await _unitOfWork.PatientPersonalInfos.GetByDoctorIdAsync(doctorId);
        var codes = await _unitOfWork.PatientCodes.GetAllAsync();
        return ServiceResult<AppointmentDto>.SuccessResult(MapToDto(appointment, patients, codes), "تم تسجيل عدم الحضور");
    }

    public async Task<ServiceResult<bool>> DeleteAppointmentAsync(Guid id, Guid doctorId)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAndDoctorAsync(id, doctorId);
        if (appointment is null) return ServiceResult<bool>.NotFoundResult("الموعد غير موجود");

        await _unitOfWork.Appointments.DeleteAsync(appointment);
        return ServiceResult<bool>.SuccessResult(true, "تم حذف الموعد");
    }

    private static AppointmentDto MapToDto(
        Appointment appointment,
        IEnumerable<PatientPersonalInfo> patients,
        IEnumerable<PatientCode> codes)
    {
        var patient = patients.FirstOrDefault(p => p.Id == appointment.PatientId);
        var code = codes.FirstOrDefault(c => c.PatientId == appointment.PatientId);

        return new AppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "غير معروف",
            PatientCode = code?.Code ?? "—",
            ScheduledDate = appointment.ScheduledDate,
            ScheduledTime = appointment.ScheduledTime,
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            VisitType = appointment.VisitType,
            VisitLocation = appointment.VisitLocation,
            Status = appointment.Status,
            CancellationReason = appointment.CancellationReason,
            CancelledAt = appointment.CancelledAt,
            VisitId = appointment.VisitId,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
}