using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FamilyHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Relation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelativeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConditionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyHistories_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FamilyHistories_DoctorId",
                table: "FamilyHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyHistories_PatientId",
                table: "FamilyHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyHistories_PatientId_IsDeleted",
                table: "FamilyHistories",
                columns: new[] { "PatientId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamilyHistories");
        }
    }
}
