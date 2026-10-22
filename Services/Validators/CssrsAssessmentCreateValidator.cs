using FluentValidation;
using PatientService.Services.DTOs.CssrsAssessment;

namespace PatientService.Services.Validators;

public class CssrsAssessmentCreateValidator : AbstractValidator<CssrsAssessmentCreateDto>
{
    private static readonly string[] AllowedSeverities = { "low", "moderate", "high", "critical" };

    public CssrsAssessmentCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.TotalScore)
            .InclusiveBetween(0, 6).WithMessage("النتيجة الإجمالية يجب أن تكون بين 0 و 6");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("تصنيف الخطر مطلوب")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لتصنيف الخطر");

        RuleFor(x => x.AnswersJson)
            .MaximumLength(2000).WithMessage("حجم الإجابات كبير جداً")
            .When(x => !string.IsNullOrWhiteSpace(x.AnswersJson));
    }
}