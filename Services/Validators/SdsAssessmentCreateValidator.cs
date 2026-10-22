using FluentValidation;
using PatientService.Services.DTOs.SdsAssessment;

namespace PatientService.Services.Validators;

public class SdsAssessmentCreateValidator : AbstractValidator<SdsAssessmentCreateDto>
{
    private static readonly string[] AllowedSeverities = { "minimal", "mild", "moderate", "severe" };

    public SdsAssessmentCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.TotalScore)
            .InclusiveBetween(0, 30).WithMessage("النتيجة الإجمالية يجب أن تكون بين 0 و 30");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("تصنيف الشدة مطلوب")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لتصنيف الشدة");

        RuleFor(x => x.AnswersJson)
            .MaximumLength(2000).WithMessage("حجم الإجابات كبير جداً")
            .When(x => !string.IsNullOrWhiteSpace(x.AnswersJson));
    }
}