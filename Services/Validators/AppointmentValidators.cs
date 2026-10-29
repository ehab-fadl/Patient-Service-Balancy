using FluentValidation;
using PatientService.Services.DTOs.Appointments;

namespace PatientService.Services.Validators;

public class AppointmentCreateValidator : AbstractValidator<AppointmentCreateDto>
{
    public AppointmentCreateValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("تاريخ الموعد مطلوب")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("لا يمكن حجز موعد في تاريخ سابق");

        RuleFor(x => x.ScheduledTime)
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .When(x => !string.IsNullOrWhiteSpace(x.ScheduledTime))
            .WithMessage("صيغة الوقت غير صحيحة (مثال: 14:30)");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("سبب الموعد مطلوب")
            .MaximumLength(200).WithMessage("السبب يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public class AppointmentUpdateValidator : AbstractValidator<AppointmentUpdateDto>
{
    public AppointmentUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الموعد مطلوب");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("تاريخ الموعد مطلوب");

        RuleFor(x => x.ScheduledTime)
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .When(x => !string.IsNullOrWhiteSpace(x.ScheduledTime))
            .WithMessage("صيغة الوقت غير صحيحة");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("سبب الموعد مطلوب");
    }
}

public class AppointmentCancelValidator : AbstractValidator<AppointmentCancelDto>
{
    public AppointmentCancelValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الموعد مطلوب");

        RuleFor(x => x.CancellationReason)
            .NotEmpty().WithMessage("سبب الإلغاء مطلوب")
            .MaximumLength(500).WithMessage("السبب يجب ألا يتجاوز 500 حرف");
    }
}