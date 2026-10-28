namespace PatientService.Services.DTOs.Statistics;

public class DailyMetricSubmitDto
{
    public DateTime LogDate { get; set; }
    public int Food { get; set; }
    public decimal SleepHours { get; set; }
    public int MedicationAdherence { get; set; }
    public int Mood { get; set; }
    public int NegativeThoughts { get; set; }
    public int PhysicalHealth { get; set; }
    public int Energy { get; set; }
    public int Motivation { get; set; }
    public int Concentration { get; set; }
    public int Guilt { get; set; }
    public int Hope { get; set; }
    public int SuicidalIdeation { get; set; }
}