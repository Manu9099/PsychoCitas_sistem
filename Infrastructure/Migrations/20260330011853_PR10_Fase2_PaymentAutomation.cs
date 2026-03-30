using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsychoCitas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PR10_Fase2_PaymentAutomation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntentosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PagoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Proveedor = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    ExternalReference = table.Column<string>(type: "text", nullable: true),
                    CheckoutUrl = table.Column<string>(type: "text", nullable: true),
                    ProviderPaymentId = table.Column<string>(type: "text", nullable: true),
                    RawResponse = table.Column<string>(type: "text", nullable: true),
                    ExpiraEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntentosPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntentosPago_pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "pagos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IntentoPagoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Proveedor = table.Column<int>(type: "integer", nullable: false),
                    TipoEvento = table.Column<string>(type: "text", nullable: false),
                    PayloadRaw = table.Column<string>(type: "text", nullable: false),
                    ProviderEventId = table.Column<string>(type: "text", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosPago_IntentosPago_IntentoPagoId",
                        column: x => x.IntentoPagoId,
                        principalTable: "IntentosPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosPago_IntentoPagoId",
                table: "EventosPago",
                column: "IntentoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_IntentosPago_PagoId",
                table: "IntentosPago",
                column: "PagoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosPago");

            migrationBuilder.DropTable(
                name: "IntentosPago");
        }
    }
}
