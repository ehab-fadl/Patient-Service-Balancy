using FluentValidation;
using PatientService.Services.DTOs.PatientPersonalInfo;

namespace PatientService.Services.Validators;

public class PatientPersonalInfoUpdateValidator : AbstractValidator<PatientPersonalInfoUpdateDto>
{
    private static readonly string[] AllowedGenders = { "ذكر", "أنثى" };
    private static readonly string[] AllowedMaritalStatuses = { "أعزب", "متزوج", "أرمل", "مطلق" };

    public PatientPersonalInfoUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرّف المريض مطلوب");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("الاسم الأول مطلوب")
            .MinimumLength(2).WithMessage("الاسم الأول يجب أن يكون حرفين على الأقل")
            .MaximumLength(50).WithMessage("الاسم الأول يجب ألا يتجاوز 50 حرفاً");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("اسم العائلة مطلوب")
            .MinimumLength(2).WithMessage("اسم العائلة يجب أن يكون حرفين على الأقل")
            .MaximumLength(50).WithMessage("اسم العائلة يجب ألا يتجاوز 50 حرفاً");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("تاريخ الميلاد مطلوب")
            .LessThan(DateTime.UtcNow).WithMessage("تاريخ الميلاد لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("الجنس مطلوب")
            .Must(g => AllowedGenders.Contains(g))
            .WithMessage("الجنس يجب أن يكون 'ذكر' أو 'أنثى'");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9\s\-()]{7,20}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("صيغة رقم الهاتف غير صحيحة");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("العنوان مطلوب")
            .MinimumLength(5).WithMessage("العنوان يجب أن يكون 5 أحرف على الأقل")
            .MaximumLength(300).WithMessage("العنوان يجب ألا يتجاوز 300 حرف");

        RuleFor(x => x.City)
            .MaximumLength(60).WithMessage("المدينة يجب ألا تتجاوز 60 حرفاً")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.MaritalStatus)
            .Must(s => AllowedMaritalStatuses.Contains(s))
            .When(x => !string.IsNullOrWhiteSpace(x.MaritalStatus))
            .WithMessage("الحالة الاجتماعية يجب أن تكون 'أعزب' أو 'متزوج' أو 'أرمل' أو 'مطلق'");
    }
}