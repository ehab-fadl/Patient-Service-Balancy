using FluentValidation;
using PatientService.Services.DTOs.ChronicDisease;

namespace PatientService.Services.Validators;

public class ChronicDiseaseCreateValidator : AbstractValidator<ChronicDiseaseCreateDto>
{
    public ChronicDiseaseCreateValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المرض مطلوب")
            .MinimumLength(2).WithMessage("اسم المرض يجب أن يكون حرفين على الأقل")
            .MaximumLength(100).WithMessage("اسم المرض يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.DiagnosisDate)
            .NotEmpty().WithMessage("تاريخ التشخيص مطلوب")
            .LessThan(DateTime.UtcNow.Date.AddDays(1))
                .WithMessage("تاريخ التشخيص لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Medications)
            .NotEmpty().WithMessage("الأدوية المستخدمة مطلوبة")
            .MinimumLength(2).WithMessage("يرجى كتابة اسم الدواء على الأقل")
            .MaximumLength(200).WithMessage("الأدوية يجب ألا تتجاوز 200 حرف");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("الملاحظات يجب ألا تتجاوز 500 حرف")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}