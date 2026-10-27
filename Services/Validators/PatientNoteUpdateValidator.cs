using FluentValidation;
using PatientService.Services.DTOs.PatientNote;

namespace PatientService.Services.Validators;

public class PatientNoteUpdateValidator : AbstractValidator<PatientNoteUpdateDto>
{
    private static readonly string[] AllowedTypes = { "Progress", "SOAP", "Incident", "Handoff", "Consultation", "Discharge", "Other" };
    private static readonly string[] AllowedRoles = { "طبيب", "معالج", "ممرض", "إداري", "أخرى" };
    private static readonly string[] AllowedVisibilities = { "خاص", "للفريق", "للمريض" };
    private static readonly string[] AllowedPriorities = { "عادية", "مهمة", "عاجلة" };
    private static readonly string[] AllowedRisks = { "لا", "منخفض", "متوسط", "عالي", "حرج" };

    public PatientNoteUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الملاحظة مطلوب");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان الملاحظة مطلوب")
            .MaximumLength(150).WithMessage("العنوان يجب ألا يتجاوز 150 حرف");

        RuleFor(x => x.NoteDate).NotEmpty().WithMessage("تاريخ الملاحظة مطلوب");

        RuleFor(x => x.NoteType)
            .NotEmpty().WithMessage("نوع الملاحظة مطلوب")
            .Must(t => AllowedTypes.Contains(t)).WithMessage("نوع الملاحظة غير صالح");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("كاتب الملاحظة مطلوب")
            .MaximumLength(100).WithMessage("الاسم يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.AuthorRole)
            .NotEmpty().WithMessage("دور الكاتب مطلوب")
            .Must(r => AllowedRoles.Contains(r)).WithMessage("الدور غير صالح");

        RuleFor(x => x.Priority)
            .Must(p => string.IsNullOrEmpty(p) || AllowedPriorities.Contains(p))
            .WithMessage("الأولوية غير صالحة");

        RuleFor(x => x.RiskLevel)
            .Must(r => string.IsNullOrEmpty(r) || AllowedRisks.Contains(r))
            .WithMessage("مستوى الخطر غير صالح");

        RuleFor(x => x.MoodAtNote).InclusiveBetween(0, 10).WithMessage("المزاج يجب أن يكون بين 0 و 10");

        RuleFor(x => x.Visibility)
            .NotEmpty().WithMessage("مستوى السرية مطلوب")
            .Must(v => AllowedVisibilities.Contains(v)).WithMessage("مستوى السرية غير صالح");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("محتوى الملاحظة مطلوب")
            .MinimumLength(10).WithMessage("المحتوى قصير جداً (10 أحرف على الأقل)");
    }
}