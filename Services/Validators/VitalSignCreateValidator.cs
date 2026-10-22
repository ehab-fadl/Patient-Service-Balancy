using FluentValidation;
using PatientService.Services.DTOs.VitalSign;

namespace PatientService.Services.Validators;

public class VitalSignCreateValidator : AbstractValidator<VitalSignCreateDto>
{
    private static readonly string[] AllowedTypes = {
        "bloodPressure", "heartRate", "weight", "height", "bmi",
        "glucose", "temperature", "oxygen", "respiratory", "waist"
    };
    private static readonly string[] AllowedStatuses = { "normal", "warning", "danger" };

    public VitalSignCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.VitalType)
            .NotEmpty().WithMessage("نوع القياس مطلوب")
            .Must(t => AllowedTypes.Contains(t)).WithMessage("نوع القياس غير صالح");

        RuleFor(x => x.MeasurementDate)
            .NotEmpty().WithMessage("تاريخ القياس مطلوب")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5)).WithMessage("تاريخ القياس لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Value1)
            .NotEmpty().WithMessage("القيمة الأولى مطلوبة");

        RuleFor(x => x.Value2)
            .NotNull().WithMessage("القيمة الثانية (الانبساطي) مطلوبة لقياس الضغط")
            .When(x => x.VitalType == "bloodPressure");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("تقييم الحالة مطلوب")
            .Must(s => AllowedStatuses.Contains(s)).WithMessage("قيمة غير صالحة للحالة");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}