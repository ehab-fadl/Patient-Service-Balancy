using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddListeningSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListeningSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    SessionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TherapyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TherapistName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LinkedTreatmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoodBefore = table.Column<int>(type: "int", nullable: false),
                    MoodAfter = table.Column<int>(type: "int", nullable: false),
                    TopicsDiscussed = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TechniquesUsed = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RiskLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HomeworkCompleted = table.Column<bool>(type: "bit", nullable: false),
                    HomeworkAssigned = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NextSessionPlan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningSessions_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListeningSessions_DoctorId",
                table: "ListeningSessions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningSessions_PatientId",
                table: "ListeningSessions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningSessions_PatientId_IsDeleted",
                table: "ListeningSessions",
                columns: new[] { "PatientId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListeningSessions");
        }
    }
}
