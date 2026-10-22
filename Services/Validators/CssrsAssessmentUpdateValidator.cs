using FluentValidation;
using PatientService.Services.DTOs.CssrsAssessment;

namespace PatientService.Services.Validators;

public class CssrsAssessmentUpdateValidator : AbstractValidator<CssrsAssessmentUpdateDto>
{
    private static readonly string[] AllowedSeverities = { "low", "moderate", "high", "critical" };

    public CssrsAssessmentUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف التقييم مطلوب");

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