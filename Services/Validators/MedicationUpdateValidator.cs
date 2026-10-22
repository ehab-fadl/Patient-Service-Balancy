using FluentValidation;
using PatientService.Services.DTOs.Medication;

namespace PatientService.Services.Validators;

public class MedicationUpdateValidator : AbstractValidator<MedicationUpdateDto>
{
    private static readonly string[] AllowedFrequencies = { "مرة واحدة يومياً", "مرتين يومياً", "ثلاث مرات يومياً", "أربع مرات يومياً", "عند الحاجة", "أسبوعياً", "شهرياً" };
    private static readonly string[] AllowedPeriods = { "صباحاً", "ظهراً", "مساءً", "ليلاً" };
    private static readonly string[] AllowedRoutes = { "فموي (أقراص)", "فموي (شراب)", "تحت اللسان", "حقن عضلي", "حقن وريدي", "حقن تحت الجلد", "موضعي", "استنشاق", "تحاميل" };
    private static readonly string[] AllowedStatuses = { "نشط", "متوقف", "مكتمل", "معلق مؤقتاً" };

    public MedicationUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الدواء مطلوب");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم الدواء مطلوب")
            .MaximumLength(120).WithMessage("اسم الدواء يجب ألا يتجاوز 120 حرف");

        RuleFor(x => x.Dose)
            .NotEmpty().WithMessage("الجرعة مطلوبة")
            .MaximumLength(80).WithMessage("الجرعة يجب ألا تتجاوز 80 حرف");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("التكرار مطلوب")
            .Must(f => AllowedFrequencies.Contains(f)).WithMessage("قيمة غير صالحة للتكرار");

        RuleFor(x => x.IntakePeriod)
            .NotEmpty().WithMessage("فترة التناول مطلوبة")
            .Must(p => AllowedPeriods.Contains(p)).WithMessage("قيمة غير صالحة لفترة التناول");

        RuleFor(x => x.Route)
            .NotEmpty().WithMessage("طريقة الإعطاء مطلوبة")
            .Must(r => AllowedRoutes.Contains(r)).WithMessage("قيمة غير صالحة لطريقة الإعطاء");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البدء مطلوب");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("الحالة مطلوبة")
            .Must(s => AllowedStatuses.Contains(s)).WithMessage("قيمة غير صالحة للحالة");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}