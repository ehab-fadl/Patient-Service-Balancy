using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsLegal = table.Column<bool>(type: "bit", nullable: false),
                    PatientNameSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PatientDobSnapshot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientGenderSnapshot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PatientDiagnosisSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FormDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientReports_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientReports_DoctorId",
                table: "PatientReports",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientReports_DoctorId_ReportType",
                table: "PatientReports",
                columns: new[] { "DoctorId", "ReportType" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientReports_PatientId",
                table: "PatientReports",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientReports_PatientId_IsDeleted",
                table: "PatientReports",
                columns: new[] { "PatientId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientReports");
        }
    }
}
