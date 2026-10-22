using FluentValidation;
using PatientService.Services.DTOs.MdqAssessment;

namespace PatientService.Services.Validators;

public class MdqAssessmentCreateValidator : AbstractValidator<MdqAssessmentCreateDto>
{
    private static readonly string[] AllowedSeverities = { "positive", "negative" };

    public MdqAssessmentCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.TotalScore)
            .InclusiveBetween(0, 13).WithMessage("النتيجة الإجمالية يجب أن تكون بين 0 و 13");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("تصنيف الاحتمالية مطلوب")
            .Must(s => AllowedSeverities.Contains(s)).WithMessage("قيمة غير صالحة لتصنيف الاحتمالية");

        RuleFor(x => x.AnswersJson)
            .MaximumLength(2000).WithMessage("حجم الإجابات كبير جداً")
            .When(x => !string.IsNullOrWhiteSpace(x.AnswersJson));
    }
}