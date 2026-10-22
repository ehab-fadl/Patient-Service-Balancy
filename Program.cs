using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Data.Repositories;
using PatientService.Models.Interfaces;
using PatientService.Services.Factories;
using PatientService.Services.Implementations;
using PatientService.Services.Interfaces;
using PatientService.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
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
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>(); // ← جديد

// Services
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
builder.Services.AddScoped<IMedicationService, MedicationService>(); // ← جديد

// Factories
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
builder.Services.AddScoped<IMedicationFactory, MedicationFactory>(); // ← جديد

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PatientPersonalInfoCreateValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();