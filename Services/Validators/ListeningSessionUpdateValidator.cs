using FluentValidation;
using PatientService.Services.DTOs.ListeningSession;

namespace PatientService.Services.Validators;

public class ListeningSessionUpdateValidator : AbstractValidator<ListeningSessionUpdateDto>
{
    private static readonly string[] AllowedStatuses = { "مكتملة", "مجدولة", "ملغية", "مؤجلة", "لم يحضر" };
    private static readonly string[] AllowedRisks = { "منخفض", "متوسط", "عالي", "حرج" };

    public ListeningSessionUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرّف الجلسة مطلوب");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان الجلسة مطلوب")
            .MaximumLength(150).WithMessage("العنوان يجب ألا يتجاوز 150 حرف");

        RuleFor(x => x.SessionDate).NotEmpty().WithMessage("تاريخ الجلسة مطلوب");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 480).When(x => x.DurationMinutes.HasValue)
            .WithMessage("المدة يجب أن تكون بين 1 و 480 دقيقة");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("حالة الجلسة مطلوبة")
            .Must(s => AllowedStatuses.Contains(s)).WithMessage("حالة الجلسة غير صالحة");

        RuleFor(x => x.MoodBefore).InclusiveBetween(0, 10).WithMessage("المزاج قبل الجلسة يجب أن يكون بين 0 و 10");
        RuleFor(x => x.MoodAfter).InclusiveBetween(0, 10).WithMessage("المزاج بعد الجلسة يجب أن يكون بين 0 و 10");

        RuleFor(x => x.RiskLevel)
            .Must(r => string.IsNullOrEmpty(r) || AllowedRisks.Contains(r))
            .WithMessage("مستوى الخطر غير صالح");
    }
}