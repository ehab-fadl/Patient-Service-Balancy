using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientCode",
                table: "PatientPersonalInfos");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "PatientPersonalInfos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PatientPersonalInfos_DoctorId",
                table: "PatientPersonalInfos",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientPersonalInfos_DoctorId",
                table: "PatientPersonalInfos");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "PatientPersonalInfos");

            migrationBuilder.AddColumn<string>(
                name: "PatientCode",
                table: "PatientPersonalInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
