using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PsychoCitas.Domain.Enums;

#nullable disable

namespace PsychoCitas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_CitasNotasAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:estado_cita", "programada,confirmada,completada,cancelada,no_asistio,reagendada")
                .Annotation("Npgsql:Enum:modalidad", "presencial,videollamada,telefonica")
                .Annotation("Npgsql:Enum:tipo_sesion", "individual,pareja,familia,grupo,evaluacion,seguimiento");

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    genero = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    dni = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telefono_emergencia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contacto_emergencia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ocupacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado_civil = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    referido_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    paciente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    psicologo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo_sesion = table.Column<TipoSesion>(type: "tipo_sesion", nullable: false),
                    modalidad = table.Column<Modalidad>(type: "modalidad", nullable: false),
                    estado = table.Column<EstadoCita>(type: "estado_cita", nullable: false),
                    link_videollamada = table.Column<string>(type: "text", nullable: true),
                    numero_sesion = table.Column<int>(type: "integer", nullable: true),
                    es_primera_vez = table.Column<bool>(type: "boolean", nullable: false),
                    notas_previas = table.Column<string>(type: "text", nullable: true),
                    motivo_cancelacion = table.Column<string>(type: "text", nullable: true),
                    cancelado_por = table.Column<Guid>(type: "uuid", nullable: true),
                    cancelado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    recurrencia_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_por = table.Column<Guid>(type: "uuid", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.id);
                    table.ForeignKey(
                        name: "FK_citas_pacientes_paciente_id",
                        column: x => x.paciente_id,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historia_clinica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    paciente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    psicologo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    motivo_consulta = table.Column<string>(type: "text", nullable: true),
                    diagnostico_inicial = table.Column<string>(type: "text", nullable: true),
                    diagnostico_cie11 = table.Column<string>(type: "text", nullable: true),
                    antecedentes_personales = table.Column<string>(type: "text", nullable: true),
                    antecedentes_familiares = table.Column<string>(type: "text", nullable: true),
                    medicacion_actual = table.Column<string>(type: "text", nullable: true),
                    alergias = table.Column<string>(type: "text", nullable: true),
                    tratamientos_previos = table.Column<string>(type: "text", nullable: true),
                    objetivos_terapeuticos = table.Column<string>(type: "text", nullable: true),
                    observaciones_iniciales = table.Column<string>(type: "text", nullable: true),
                    fecha_ingreso = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_alta = table.Column<DateOnly>(type: "date", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historia_clinica", x => x.id);
                    table.ForeignKey(
                        name: "FK_historia_clinica_pacientes_paciente_id",
                        column: x => x.paciente_id,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notas_sesion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cita_id = table.Column<Guid>(type: "uuid", nullable: false),
                    psicologo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resumen_sesion = table.Column<string>(type: "text", nullable: true),
                    tecnicas_usadas = table.Column<List<string>>(type: "text[]", nullable: false),
                    estado_animo = table.Column<int>(type: "integer", nullable: true),
                    nivel_ansiedad = table.Column<int>(type: "integer", nullable: true),
                    avance_objetivos = table.Column<string>(type: "text", nullable: true),
                    tareas_asignadas = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    plan_proxima_sesion = table.Column<string>(type: "text", nullable: true),
                    evaluacion_riesgo = table.Column<bool>(type: "boolean", nullable: false),
                    nivel_riesgo = table.Column<int>(type: "integer", nullable: true),
                    acciones_riesgo = table.Column<string>(type: "text", nullable: true),
                    finalizada = table.Column<bool>(type: "boolean", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notas_sesion", x => x.id);
                    table.ForeignKey(
                        name: "FK_notas_sesion_citas_cita_id",
                        column: x => x.cita_id,
                        principalTable: "citas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CitaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Canal = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    ProgramadaPara = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EnviadaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Mensaje = table.Column<string>(type: "text", nullable: true),
                    ErrorDetalle = table.Column<string>(type: "text", nullable: true),
                    Intentos = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "citas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cita_id = table.Column<Guid>(type: "uuid", nullable: false),
                    paciente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto = table.Column<decimal>(type: "numeric", nullable: false),
                    monto_pagado = table.Column<decimal>(type: "numeric", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    metodo_pago = table.Column<string>(type: "text", nullable: true),
                    numero_operacion = table.Column<string>(type: "text", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    registrado_por = table.Column<Guid>(type: "uuid", nullable: true),
                    pagado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id);
                    table.ForeignKey(
                        name: "FK_pagos_citas_cita_id",
                        column: x => x.cita_id,
                        principalTable: "citas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_citas_paciente_id",
                table: "citas",
                column: "paciente_id");

            migrationBuilder.CreateIndex(
                name: "IX_historia_clinica_paciente_id",
                table: "historia_clinica",
                column: "paciente_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notas_sesion_cita_id",
                table: "notas_sesion",
                column: "cita_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_CitaId",
                table: "Notificaciones",
                column: "CitaId");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_apellidos_nombres",
                table: "pacientes",
                columns: new[] { "apellidos", "nombres" });

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_dni",
                table: "pacientes",
                column: "dni",
                unique: true,
                filter: "dni IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_email",
                table: "pacientes",
                column: "email",
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_cita_id",
                table: "pagos",
                column: "cita_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historia_clinica");

            migrationBuilder.DropTable(
                name: "notas_sesion");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "pacientes");
        }
    }
}
