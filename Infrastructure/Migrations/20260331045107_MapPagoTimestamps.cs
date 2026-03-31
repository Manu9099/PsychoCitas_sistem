using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsychoCitas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MapPagoTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "actualizado_en",
                table: "pagos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "creado_en",
                table: "pagos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actualizado_en",
                table: "pagos");

            migrationBuilder.DropColumn(
                name: "creado_en",
                table: "pagos");
        }
    }
}
