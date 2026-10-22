using FluentValidation;
using PatientService.Services.DTOs.FamilyHistory;

namespace PatientService.Services.Validators;

public class FamilyHistoryCreateValidator : AbstractValidator<FamilyHistoryCreateDto>
{
    private static readonly string[] AllowedSeverities = { "unknown", "low", "medium", "high" };
    private static readonly string[] AllowedConditionTypes = { "genetic", "psych", "chronic", "other" };
    private static readonly string[] AllowedStatuses = { "حي", "متوفى", "غير معروف" };

    public FamilyHistoryCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.Relation)
            .NotEmpty().WithMessage("صلة القرابة مطلوبة")
            .MaximumLength(50).WithMessage("صلة القرابة طويلة جداً");

        RuleFor(x => x.RelativeName)
            .MaximumLength(100).WithMessage("اسم الفرد يجب ألا يتجاوز 100 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.RelativeName));

        RuleFor(x => x.Age)
            .InclusiveBetween(0, 120).WithMessage("العمر يجب أن يكون بين 0 و 120")
            .When(x => x.Age.HasValue);

        RuleFor(x => x.Status)
            .Must(s => AllowedStatuses.Contains(s))
            .When(x => !string.IsNullOrWhiteSpace(x.Status))
            .WithMessage("قيمة غير صالحة للحالة");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("شدة التأثير مطلوبة")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لشدة التأثير");

        RuleFor(x => x.ConditionType)
            .NotEmpty().WithMessage("نوع المرض مطلوب")
            .Must(t => AllowedConditionTypes.Contains(t)).WithMessage("قيمة غير صالحة لنوع المرض");

        RuleFor(x => x.Conditions)
            .NotEmpty().WithMessage("الأمراض / الحالات مطلوبة")
            .MaximumLength(500).WithMessage("الأمراض يجب ألا تتجاوز 500 حرف");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}