using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PatientService.Helpers;

/// <summary>
/// مسؤول عن تعمية كل البيانات الحساسة قبل إرسالها إلى Data Warehouse.
/// </summary>
public static class DeidentificationHelper
{
    private static string _salt = "CHANGE_ME_IN_CONFIG";

    public static void Initialize(IConfiguration config)
    {
        _salt = config["Etl:HashingSalt"]
            ?? throw new InvalidOperationException("Etl:HashingSalt not configured");
    }

    // ═══════════════════════════════════════════════════════════════
    // 1. Hashing
    // ═══════════════════════════════════════════════════════════════

    public static string HashId(Guid id)
    {
        var input = $"{_salt}:{id}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
    }

    public static string HashString(string value)
    {
        var input = $"{_salt}:{value}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. Generalization
    // ═══════════════════════════════════════════════════════════════

    public static string GeneralizeAge(DateTime dateOfBirth)
    {
        var age = DateTime.UtcNow.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.UtcNow.AddYears(-age)) age--;

        return age switch
        {
            < 18 => "under_18",
            <= 25 => "18-25",
            <= 35 => "26-35",
            <= 45 => "36-45",
            <= 60 => "46-60",
            <= 75 => "61-75",
            _ => "over_75"
        };
    }

    public static string GeneralizeQuarter(DateTime date)
    {
        var quarter = (date.Month - 1) / 3 + 1;
        return $"{date.Year}-Q{quarter}";
    }

    public static string GeneralizeMonth(DateTime date)
    {
        return date.ToString("yyyy-MM");
    }

    public static int CalculateDurationDays(DateTime start, DateTime? end)
    {
        return (int)((end ?? DateTime.UtcNow) - start).TotalDays;
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. Category Mapping
    // ═══════════════════════════════════════════════════════════════

    public static string MapGender(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender)) return "unknown";
        return gender.Trim() switch
        {
            "ذكر" or "male" or "Male" or "M" => "male",
            "أنثى" or "female" or "Female" or "F" => "female",
            _ => "unknown"
        };
    }

    public static string MapRegion(string? city)
    {
        if (string.IsNullOrWhiteSpace(city)) return "unknown";

        var regionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "برلين", "berlin" },
            { "هامبورغ", "hamburg" },
            { "ميونخ", "munich" },
            { "كولونيا", "cologne" },
            { "فرانكفورت", "frankfurt" },
            { "شتوتغارت", "stuttgart" },
        };

        return regionMap.TryGetValue(city.Trim(), out var mapped) ? mapped : "other";
    }

    public static string MapMaritalStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return "unknown";
        return status.Trim() switch
        {
            "أعزب" or "single" => "single",
            "متزوج" or "married" => "married",
            "أرمل" or "widowed" => "widowed",
            "مطلق" or "divorced" => "divorced",
            _ => "unknown"
        };
    }

    public static string MapMedicationClass(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "unknown";
        var lower = name.ToLowerInvariant();

        return lower switch
        {
            var s when s.Contains("سيرترالين") || s.Contains("فلوكستين")
                    || s.Contains("باروكستين") || s.Contains("سيتالوبرام") => "SSRI",
            var s when s.Contains("فينلافاكسين") || s.Contains("دولوكستين") => "SNRI",
            var s when s.Contains("أميتريبتيلين") || s.Contains("نورتريبتيلين") => "TCA",
            var s when s.Contains("ميرتازابين") => "NaSSA",
            var s when s.Contains("بوبروبيون") => "NDRI",
            var s when s.Contains("ألبرازولام") || s.Contains("ديازيبام") => "Benzodiazepine",
            var s when s.Contains("أريبيبرازول") || s.Contains("أولانزابين") => "Atypical Antipsychotic",
            var s when s.Contains("لاموتريجين") || s.Contains("فالبروات") => "Mood Stabilizer",
            _ => "other"
        };
    }

    public static string MapDiagnosisSeverity(string? severity)
    {
        if (string.IsNullOrWhiteSpace(severity)) return "unknown";
        return severity.Trim() switch
        {
            "بسيط" => "mild",
            "متوسط" => "moderate",
            "شديد" => "severe",
            "حرج" => "critical",
            _ => "unknown"
        };
    }

    public static string MapTreatmentResponse(string? response)
    {
        if (string.IsNullOrWhiteSpace(response)) return "unknown";
        return response.Trim() switch
        {
            "ممتازة" => "excellent",
            "جيدة" => "good",
            "جزئية" => "partial",
            "ضعيفة / مقاومة" => "resistant",
            _ => "unknown"
        };
    }

    public static string MapConditionType(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "other";
        var lower = name.ToLowerInvariant();

        return lower switch
        {
            var s when s.Contains("ضغط") || s.Contains("hypertension") => "hypertension",
            var s when s.Contains("سكري") || s.Contains("diabetes") => "diabetes",
            var s when s.Contains("قلب") || s.Contains("heart") => "cardiac",
            var s when s.Contains("ربو") || s.Contains("asthma") => "asthma",
            var s when s.Contains("كلى") || s.Contains("kidney") => "kidney",
            var s when s.Contains("سرطان") || s.Contains("cancer") => "cancer",
            _ => "other"
        };
    }

    public static string MapVisitType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type)) return "unknown";
        return type.Trim() switch
        {
            "أولى" => "initial",
            "متابعة" => "followup",
            "طارئة" => "emergency",
            "استشارة سريعة" => "consultation",
            _ => "other"
        };
    }

    public static string MapTherapyType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type)) return "unknown";
        var lower = type.ToLowerInvariant();
        if (lower.Contains("cbt")) return "CBT";
        if (lower.Contains("dbt")) return "DBT";
        if (lower.Contains("act")) return "ACT";
        if (lower.Contains("دعمي")) return "supportive";
        return "other";
    }

    public static string MapVitalType(string type)
    {
        return type switch
        {
            "bloodPressure" => "blood_pressure",
            "heartRate" => "heart_rate",
            "weight" => "weight",
            "height" => "height",
            "bmi" => "bmi",
            "glucose" => "glucose",
            "temperature" => "temperature",
            "oxygen" => "oxygen_saturation",
            "respiratory" => "respiratory_rate",
            "waist" => "waist_circumference",
            _ => "other"
        };
    }

    public static string GeneralizeDoseCategory(string? dose)
    {
        if (string.IsNullOrWhiteSpace(dose)) return "unknown";
        var lower = dose.ToLowerInvariant();
        if (lower.Contains("5") || lower.Contains("10")) return "low";
        if (lower.Contains("20") || lower.Contains("25") || lower.Contains("50")) return "standard";
        if (lower.Contains("100") || lower.Contains("150") || lower.Contains("200")) return "high";
        return "standard";
    }

    public static string MapRiskLevel(string? level)
    {
        if (string.IsNullOrWhiteSpace(level)) return "unknown";
        return level.ToLowerInvariant() switch
        {
            "low" or "minimal" => "low",
            "moderate" or "medium" => "moderate",
            "high" => "high",
            "critical" => "critical",
            _ => "unknown"
        };
    }

    // ═══════════════════════════════════════════════════════════════
    // 4. المحدثات الجديدة
    // ═══════════════════════════════════════════════════════════════

    public static string MapFrequency(string? frequency)
    {
        if (string.IsNullOrWhiteSpace(frequency)) return "unknown";
        return frequency.Trim() switch
        {
            "مرة واحدة يومياً" => "once_daily",
            "مرتين يومياً" => "twice_daily",
            "ثلاث مرات يومياً" => "three_times_daily",
            "أربع مرات يومياً" => "four_times_daily",
            "عند الحاجة" => "as_needed",
            "أسبوعياً" => "weekly",
            "شهرياً" => "monthly",
            _ => "other"
        };
    }

    public static string MapMedicationStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return "unknown";
        return status.Trim() switch
        {
            "نشط" => "active",
            "متوقف" => "discontinued",
            "مكتمل" => "completed",
            "معلق مؤقتاً" => "suspended",
            _ => "other"
        };
    }

    public static string MapVisitLocation(string? location)
    {
        if (string.IsNullOrWhiteSpace(location)) return "unknown";
        return location.Trim() switch
        {
            "عيادة" => "clinic",
            "منزل" => "home",
            "أونلاين" => "online",
            "مستشفى" => "hospital",
            _ => "other"
        };
    }

    public static string MapRelation(string? relation)
    {
        if (string.IsNullOrWhiteSpace(relation)) return "unknown";
        var lower = relation.Trim().ToLowerInvariant();
        return lower switch
        {
            var s when s.Contains("أب") || s.Contains("father") => "father",
            var s when s.Contains("أم") || s.Contains("mother") => "mother",
            var s when s.Contains("أخ") || s.Contains("brother") => "brother",
            var s when s.Contains("أخت") || s.Contains("sister") => "sister",
            _ => "other"
        };
    }

    public static string MapSubstanceType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type)) return "unknown";
        var lower = type.ToLowerInvariant();
        return lower switch
        {
            var s when s.Contains("تبغ") || s.Contains("سجائر") || s.Contains("tobacco") => "tobacco",
            var s when s.Contains("كحول") || s.Contains("alcohol") => "alcohol",
            var s when s.Contains("حشيش") || s.Contains("cannabis") => "cannabis",
            var s when s.Contains("أفيون") || s.Contains("opioid") => "opioids",
            _ => "other"
        };
    }

    public static string MapSubstanceStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return "unknown";
        return status.Trim() switch
        {
            "نشط" => "active",
            "في remission" => "in_remission",
            "متوقف" => "discontinued",
            _ => "other"
        };
    }

    public static string MapPlanStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return "unknown";
        return status.Trim() switch
        {
            "نشطة" => "active",
            "مكتملة" => "completed",
            "ملغاة" => "cancelled",
            _ => "other"
        };
    }

    public static string MapProblemCategory(string? problemText)
    {
        if (string.IsNullOrWhiteSpace(problemText)) return "other";
        var lower = problemText.ToLowerInvariant();
        return lower switch
        {
            var s when s.Contains("قلق") || s.Contains("anxiety") => "Anxiety",
            var s when s.Contains("اكتئاب") || s.Contains("depress") => "Depression",
            var s when s.Contains("نوم") || s.Contains("sleep") => "Sleep Disturbance",
            var s when s.Contains("غضب") || s.Contains("anger") => "Anger Management",
            _ => "other"
        };
    }
}