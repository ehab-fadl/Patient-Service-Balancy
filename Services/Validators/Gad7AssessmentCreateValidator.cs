using FluentValidation;
using PatientService.Services.DTOs.Gad7Assessment;

namespace PatientService.Services.Validators;

public class Gad7AssessmentCreateValidator : AbstractValidator<Gad7AssessmentCreateDto>
{
    private static readonly string[] AllowedSeverities = { "minimal", "mild", "moderate", "severe" };

    public Gad7AssessmentCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

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