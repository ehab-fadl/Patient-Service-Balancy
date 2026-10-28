using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RefNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDocuments_PatientPersonalInfos_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientPersonalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocuments_DoctorId",
                table: "PatientDocuments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocuments_PatientId",
                table: "PatientDocuments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocuments_PatientId_IsDeleted",
                table: "PatientDocuments",
                columns: new[] { "PatientId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDocuments");
        }
    }
}
