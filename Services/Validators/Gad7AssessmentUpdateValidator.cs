using FluentValidation;
using PatientService.Services.DTOs.Gad7Assessment;

namespace PatientService.Services.Validators;

public class Gad7AssessmentUpdateValidator : AbstractValidator<Gad7AssessmentUpdateDto>
{
    private static readonly string[] AllowedSeverities = { "minimal", "mild", "moderate", "severe" };

    public Gad7AssessmentUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف التقييم مطلوب");

        RuleFor(x => x.TotalScore)
            .InclusiveBetween(0, 21).WithMessage("النتيجة الإجمالية يجب أن تكون بين 0 و 21");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("تصنيف الشدة مطلوب")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لتصنيف الشدة");

        RuleFor(x => x.AnswersJson)
            .MaximumLength(2000).WithMessage("حجم الإجابات كبير جداً")
            .When(x => !string.IsNullOrWhiteSpace(x.AnswersJson));
    }
}