using FluentValidation;
using PatientService.Services.DTOs.Diagnosis;

namespace PatientService.Services.Validators;

public class DiagnosisCreateValidator : AbstractValidator<DiagnosisCreateDto>
{
    private static readonly string[] AllowedSeverities = { "بسيط", "متوسط", "شديد", "حرج" };

    public DiagnosisCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.MainType)
            .NotEmpty().WithMessage("نوع الاكتئاب الرئيسي مطلوب")
            .MaximumLength(100).WithMessage("النوع الرئيسي يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.SubType)
            .NotEmpty().WithMessage("النوع الفرعي مطلوب")
            .MaximumLength(100).WithMessage("النوع الفرعي يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.DiagnosisDate)
            .NotEmpty().WithMessage("تاريخ التشخيص مطلوب")
            .LessThan(DateTime.UtcNow.Date.AddDays(1)).WithMessage("تاريخ التشخيص لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("شدة التشخيص مطلوبة")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة للشدة");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}