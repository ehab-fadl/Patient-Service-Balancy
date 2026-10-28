using PatientService.Models.Common;

namespace PatientService.Models.Entities;

public class DailyPatientMetric : BaseEntity
{
    public Guid PatientId { get; set; }
    public DateTime LogDate { get; set; } // تاريخ اليوم (بدون وقت)

    // المؤشرات الأصلية (1-6)
    public int Food { get; set; }           // 0-100%
    public decimal SleepHours { get; set; } // ساعات النوم
    public int MedicationAdherence { get; set; } // 0-100%
    public int Mood { get; set; }           // 1-10
    public int NegativeThoughts { get; set; } // عدد الأفكار
    public int PhysicalHealth { get; set; } // 0-100%

    // مؤشرات الاكتئاب الأساسية (7-12)
    public int Energy { get; set; }         // 0-10
    public int Motivation { get; set; }     // 0-10
    public int Concentration { get; set; }  // 0-10
    public int Guilt { get; set; }          // 0-10
    public int Hope { get; set; }           // 0-10
    public int SuicidalIdeation { get; set; } // 0-5

    // Navigation Property
    public PatientPersonalInfo Patient { get; set; } = null!;
}