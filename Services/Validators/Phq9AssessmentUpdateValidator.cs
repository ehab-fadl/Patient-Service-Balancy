using FluentValidation;
using PatientService.Services.DTOs.Phq9Assessment;

namespace PatientService.Services.Validators;

public class Phq9AssessmentUpdateValidator : AbstractValidator<Phq9AssessmentUpdateDto>
{
    private static readonly string[] AllowedSeverities = { "minimal", "mild", "moderate", "moderately_severe", "severe" };

    public Phq9AssessmentUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف التقييم مطلوب");

        RuleFor(x => x.TotalScore)
            .InclusiveBetween(0, 27).WithMessage("النتيجة الإجمالية يجب أن تكون بين 0 و 27");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("تصنيف الشدة مطلوب")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لتصنيف الشدة");

        RuleFor(x => x.AnswersJson)
            .MaximumLength(3000).WithMessage("حجم الإجابات كبير جداً")
            .When(x => !string.IsNullOrWhiteSpace(x.AnswersJson));
    }
}