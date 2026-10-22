using FluentValidation;
using PatientService.Services.DTOs.DepressionStory;

namespace PatientService.Services.Validators;

public class DepressionStoryUpdateValidator : AbstractValidator<DepressionStoryUpdateDto>
{
    public DepressionStoryUpdateValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.Story)
            .MinimumLength(20).WithMessage("القصة قصيرة جداً (20 حرفاً على الأقل)")
            .MaximumLength(5000).WithMessage("القصة طويلة جداً (الحد الأقصى 5000 حرف)")
            .When(x => !string.IsNullOrWhiteSpace(x.Story));
    }
}