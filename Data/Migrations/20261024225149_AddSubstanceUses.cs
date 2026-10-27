using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubstanceUses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubstanceUses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Route = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationYears = table.Column<double>(type: "float", nullable: true),
                    AgeOfOnset = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastUseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CravingLevel = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhysicalDependence = table.Column<bool>(type: "bit", nullable: false),
                    PsychologicalDependence = table.Column<bool>(type: "bit", nullable: false),
                    ImpactWork = table.Column<bool>(type: "bit", nullable: false),
                    ImpactRelationships = table.Column<bool>(type: "bit", nullable: false),
                    ImpactHealth = table.Column<bool>(type: "bit", nullable: false),
                    ImpactLegal = table.Column<bool>(type: "bit", nullable: false),
                    QuitAttempts = table.Column<int>(type: "int", nullable: true),
                    LastQuitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstanceUses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstanceUses_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubstanceUses_DoctorId",
                table: "SubstanceUses",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstanceUses_PatientId",
                table: "SubstanceUses",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstanceUses_PatientId_IsDeleted",
                table: "SubstanceUses",
                columns: new[] { "PatientId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstanceUses");
        }
    }
}
