using FluentValidation;
using PatientService.Services.DTOs.SubstanceUse;

namespace PatientService.Services.Validators;

public class SubstanceUseCreateValidator : AbstractValidator<SubstanceUseCreateDto>
{
    public SubstanceUseCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.Name).NotEmpty().WithMessage("اسم المادة مطلوب").MaximumLength(100);
        RuleFor(x => x.Type).NotEmpty().WithMessage("نوع المادة مطلوب");
        RuleFor(x => x.Route).NotEmpty().WithMessage("طريقة التعاطي مطلوبة");
        RuleFor(x => x.Frequency).NotEmpty().WithMessage("تكرار الاستخدام مطلوب");

        RuleFor(x => x.Amount).NotEmpty().WithMessage("الجرعة اليومية مطلوبة").MaximumLength(100);
        RuleFor(x => x.DurationYears).InclusiveBetween(0, 80).When(x => x.DurationYears.HasValue).WithMessage("المدة يجب أن تكون بين 0 و 80 سنة");
        RuleFor(x => x.AgeOfOnset).InclusiveBetween(5, 100).When(x => x.AgeOfOnset.HasValue).WithMessage("عمر البداية يجب أن يكون بين 5 و 100");
        RuleFor(x => x.LastUseDate).NotEmpty().WithMessage("تاريخ آخر استخدام مطلوب");
        RuleFor(x => x.CurrentStatus).NotEmpty().WithMessage("الحالة الحالية مطلوبة");

        RuleFor(x => x.CravingLevel).InclusiveBetween(0, 10).WithMessage("مستوى الرغبة يجب أن يكون بين 0 و 10");
        RuleFor(x => x.Severity).NotEmpty().WithMessage("شدة الاضطراب مطلوبة");
        RuleFor(x => x.QuitAttempts).InclusiveBetween(0, 100).When(x => x.QuitAttempts.HasValue).WithMessage("عدد المحاولات يجب أن يكون بين 0 و 100");

        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}