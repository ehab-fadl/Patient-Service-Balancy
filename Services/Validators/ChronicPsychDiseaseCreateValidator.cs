using FluentValidation;
using PatientService.Services.DTOs.ChronicPsychDisease;

namespace PatientService.Services.Validators;

public class ChronicPsychDiseaseCreateValidator : AbstractValidator<ChronicPsychDiseaseCreateDto>
{
    private static readonly string[] AllowedHospitalizations = { "نعم", "لا" };
    private static readonly string[] AllowedSeverities = { "بسيط", "متوسط", "شديد", "حرج" };
    private static readonly string[] AllowedResponses = { "ممتازة", "جيدة", "جزئية", "ضعيفة / مقاومة" };

    public ChronicPsychDiseaseCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المرض مطلوب")
            .MaximumLength(100).WithMessage("اسم المرض يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.DiagnosisDate)
            .NotEmpty().WithMessage("تاريخ التشخيص مطلوب")
            .LessThan(DateTime.UtcNow.Date.AddDays(1)).WithMessage("تاريخ التشخيص لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Hospitalization)
            .NotEmpty().WithMessage("حالة الدخول للمصح مطلوبة")
            .Must(h => AllowedHospitalizations.Contains(h)).WithMessage("قيمة غير صالحة للدخول للمصح");

        RuleFor(x => x.Medications)
            .NotEmpty().WithMessage("الأدوية مطلوبة")
            .MaximumLength(200).WithMessage("الأدوية يجب ألا تتجاوز 200 حرف");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("شدة المرض مطلوبة")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لشدة المرض");

        RuleFor(x => x.Response)
            .NotEmpty().WithMessage("الاستجابة للعلاج مطلوبة")
            .Must(r => AllowedResponses.Contains(r)).WithMessage("قيمة غير صالحة للاستجابة للعلاج");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}