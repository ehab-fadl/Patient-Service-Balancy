using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyPatientMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPatientMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Food = table.Column<int>(type: "int", nullable: false),
                    SleepHours = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    MedicationAdherence = table.Column<int>(type: "int", nullable: false),
                    Mood = table.Column<int>(type: "int", nullable: false),
                    NegativeThoughts = table.Column<int>(type: "int", nullable: false),
                    PhysicalHealth = table.Column<int>(type: "int", nullable: false),
                    Energy = table.Column<int>(type: "int", nullable: false),
                    Motivation = table.Column<int>(type: "int", nullable: false),
                    Concentration = table.Column<int>(type: "int", nullable: false),
                    Guilt = table.Column<int>(type: "int", nullable: false),
                    Hope = table.Column<int>(type: "int", nullable: false),
                    SuicidalIdeation = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPatientMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPatientMetrics_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPatientMetrics_PatientId_IsDeleted",
                table: "DailyPatientMetrics",
                columns: new[] { "PatientId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPatientMetrics_PatientId_LogDate",
                table: "DailyPatientMetrics",
                columns: new[] { "PatientId", "LogDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPatientMetrics");
        }
    }
}
