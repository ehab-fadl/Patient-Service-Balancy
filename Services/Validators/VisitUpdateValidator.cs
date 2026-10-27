using FluentValidation;
using PatientService.Services.DTOs.Visit;

namespace PatientService.Services.Validators;

public class VisitUpdateValidator : AbstractValidator<VisitUpdateDto>
{
    private static readonly string[] AllowedTypes = { "أولى", "متابعة", "طارئة", "استشارة سريعة" };
    private static readonly string[] AllowedLocations = { "عيادة", "منزل", "أونلاين", "مستشفى" };

    public VisitUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الزيارة مطلوب");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("سبب الزيارة مطلوب")
            .MaximumLength(200).WithMessage("السبب يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.VisitDate)
            .NotEmpty().WithMessage("تاريخ الزيارة مطلوب");

        RuleFor(x => x.VisitType)
            .NotEmpty().WithMessage("نوع الزيارة مطلوب")
            .Must(t => AllowedTypes.Contains(t)).WithMessage("نوع الزيارة غير صالح");

        RuleFor(x => x.VisitLocation)
            .NotEmpty().WithMessage("مكان الزيارة مطلوب")
            .Must(l => AllowedLocations.Contains(l)).WithMessage("مكان الزيارة غير صالح");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}