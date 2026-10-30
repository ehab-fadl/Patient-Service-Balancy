using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Data.Repositories;
using PatientService.Middleware;
using PatientService.Models.Interfaces;
using PatientService.Services.Factories;
using PatientService.Services.Implementations;
using PatientService.Services.Interfaces;
using PatientService.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ التعديل الأول: تعريف الـ Swagger هنا (قبل Build) ودمج الشروط
builder.Services.AddSwaggerGen(c =>
{
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        return !(apiDesc.RelativePath?.StartsWith("api/internal/etl") ?? false);
    });
});

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

PatientService.Helpers.DeidentificationHelper.Initialize(builder.Configuration);

// ─── Repositories ───
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPatientPersonalInfoRepository, PatientPersonalInfoRepository>();
builder.Services.AddScoped<IPatientCodeRepository, PatientCodeRepository>();
builder.Services.AddScoped<IDepressionStoryRepository, DepressionStoryRepository>();
builder.Services.AddScoped<IChronicDiseaseRepository, ChronicDiseaseRepository>();
builder.Services.AddScoped<IChronicPsychDiseaseRepository, ChronicPsychDiseaseRepository>();
builder.Services.AddScoped<IFamilyHistoryRepository, FamilyHistoryRepository>();
builder.Services.AddScoped<IVitalSignRepository, VitalSignRepository>();
builder.Services.AddScoped<IGad7AssessmentRepository, Gad7AssessmentRepository>();
builder.Services.AddScoped<IPhq9AssessmentRepository, Phq9AssessmentRepository>();
builder.Services.AddScoped<IMdqAssessmentRepository, MdqAssessmentRepository>();
builder.Services.AddScoped<ISdsAssessmentRepository, SdsAssessmentRepository>();
builder.Services.AddScoped<ICssrsAssessmentRepository, CssrsAssessmentRepository>();
builder.Services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<ISideEffectRepository, SideEffectRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<ISubstanceUseRepository, SubstanceUseRepository>();
builder.Services.AddScoped<IListeningSessionRepository, ListeningSessionRepository>();
builder.Services.AddScoped<IPatientNoteRepository, PatientNoteRepository>();
builder.Services.AddScoped<IDoctorNoteRepository, DoctorNoteRepository>();
builder.Services.AddScoped<IPatientDocumentRepository, PatientDocumentRepository>();
builder.Services.AddScoped<IPatientReportRepository, PatientReportRepository>();
builder.Services.AddScoped<IDailyPatientMetricRepository, DailyPatientMetricRepository>();
builder.Services.AddScoped<ITreatmentPlanRepository, TreatmentPlanRepository>();
builder.Services.AddScoped<ISystemAlertRepository, SystemAlertRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// ─── Services ───
builder.Services.AddScoped<IPatientPersonalInfoService, PatientPersonalInfoService>();
builder.Services.AddScoped<IDepressionStoryService, DepressionStoryService>();
builder.Services.AddScoped<IChronicDiseaseService, ChronicDiseaseService>();
builder.Services.AddScoped<IChronicPsychDiseaseService, ChronicPsychDiseaseService>();
builder.Services.AddScoped<IFamilyHistoryService, FamilyHistoryService>();
builder.Services.AddScoped<IVitalSignService, VitalSignService>();
builder.Services.AddScoped<IGad7AssessmentService, Gad7AssessmentService>();
builder.Services.AddScoped<IPhq9AssessmentService, Phq9AssessmentService>();
builder.Services.AddScoped<IMdqAssessmentService, MdqAssessmentService>();
builder.Services.AddScoped<ISdsAssessmentService, SdsAssessmentService>();
builder.Services.AddScoped<ICssrsAssessmentService, CssrsAssessmentService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<ISideEffectService, SideEffectService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<ISubstanceUseService, SubstanceUseService>();
builder.Services.AddScoped<IListeningSessionService, ListeningSessionService>();
builder.Services.AddScoped<IPatientNoteService, PatientNoteService>();
builder.Services.AddScoped<IDoctorNoteService, DoctorNoteService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ITreatmentPlanService, TreatmentPlanService>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IMobileService, MobileService>();
builder.Services.AddScoped<IEtlService, EtlService>();

// ─── Factories ───
builder.Services.AddScoped<IPatientPersonalInfoFactory, PatientPersonalInfoFactory>();
builder.Services.AddScoped<IPatientCodeFactory, PatientCodeFactory>();
builder.Services.AddScoped<IPatientCodeGenerator, PatientCodeGenerator>();
builder.Services.AddScoped<IDepressionStoryFactory, DepressionStoryFactory>();
builder.Services.AddScoped<IChronicDiseaseFactory, ChronicDiseaseFactory>();
builder.Services.AddScoped<IChronicPsychDiseaseFactory, ChronicPsychDiseaseFactory>();
builder.Services.AddScoped<IFamilyHistoryFactory, FamilyHistoryFactory>();
builder.Services.AddScoped<IVitalSignFactory, VitalSignFactory>();
builder.Services.AddScoped<IGad7AssessmentFactory, Gad7AssessmentFactory>();
builder.Services.AddScoped<IPhq9AssessmentFactory, Phq9AssessmentFactory>();
builder.Services.AddScoped<IMdqAssessmentFactory, MdqAssessmentFactory>();
builder.Services.AddScoped<ISdsAssessmentFactory, SdsAssessmentFactory>();
builder.Services.AddScoped<ICssrsAssessmentFactory, CssrsAssessmentFactory>();
builder.Services.AddScoped<IDiagnosisFactory, DiagnosisFactory>();
builder.Services.AddScoped<IMedicationFactory, MedicationFactory>();
builder.Services.AddScoped<ISideEffectFactory, SideEffectFactory>();
builder.Services.AddScoped<IVisitFactory, VisitFactory>();
builder.Services.AddScoped<ISubstanceUseFactory, SubstanceUseFactory>();
builder.Services.AddScoped<IListeningSessionFactory, ListeningSessionFactory>();
builder.Services.AddScoped<IPatientNoteFactory, PatientNoteFactory>();
builder.Services.AddScoped<IDoctorNoteFactory, DoctorNoteFactory>();

// ─── Validation ───
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PatientPersonalInfoCreateValidator>();

// ─── CORS ───
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ✅ بناء التطبيق (هنا تصبح الخدمات Read-Only)
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseMiddleware<PatientCodeAuthMiddleware>();
app.UseMiddleware<PatientService.Middleware.EtlAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();