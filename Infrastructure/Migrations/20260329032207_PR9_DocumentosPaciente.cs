using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsychoCitas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PR9_DocumentosPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documentos_paciente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NombreOriginal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NombreArchivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    UrlStorage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_paciente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentos_paciente_pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documentos_paciente_PacienteId",
                table: "documentos_paciente",
                column: "PacienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documentos_paciente");
        }
    }
}
