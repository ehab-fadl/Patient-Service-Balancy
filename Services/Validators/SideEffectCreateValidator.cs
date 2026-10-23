using FluentValidation;
using PatientService.Services.DTOs.SideEffect;

namespace PatientService.Services.Validators;

public class SideEffectCreateValidator : AbstractValidator<SideEffectCreateDto>
{
    public SideEffectCreateValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.MedicationName)
            .NotEmpty().WithMessage("اسم الدواء مطلوب")
            .MaximumLength(120).WithMessage("اسم الدواء يجب ألا يتجاوز 120 حرف");

        RuleFor(x => x.Symptom)
            .NotEmpty().WithMessage("الأعراض الجانبية مطلوبة")
            .MaximumLength(300).WithMessage("الأعراض يجب ألا تتجاوز 300 حرف");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}