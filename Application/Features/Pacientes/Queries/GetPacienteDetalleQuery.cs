using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Exceptions;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pacientes.Queries;

public record GetPacienteDetalleQuery(Guid PacienteId) : IRequest<PacienteDetalleDto>;

public class GetPacienteDetalleHandler(IUnitOfWork uow)
    : IRequestHandler<GetPacienteDetalleQuery, PacienteDetalleDto>
    
{
    public async Task<PacienteDetalleDto> Handle(GetPacienteDetalleQuery query, CancellationToken ct)
    {
        var paciente = await uow.Pacientes.GetConHistoriaAsync(query.PacienteId, ct)
            ?? throw new NotFoundException(nameof(Paciente), query.PacienteId);

        var citas = await uow.Citas.GetByPacienteAsync(query.PacienteId, ct);
        var citasList = citas.ToList();

       var documentos = await uow.DocumentosPaciente.GetByPacienteIdAsync(query.PacienteId, ct);


        var sesionesCompletadas = citasList.Count(c => c.Estado == EstadoCita.Completada);
        var inasistencias = citasList.Count(c => c.Estado == EstadoCita.NoAsistio);
        var ultimaSesion = citasList
            .Where(c => c.Estado == EstadoCita.Completada)
            .OrderByDescending(c => c.FechaInicio)
            .FirstOrDefault()?.FechaInicio;

        var deudaPendiente = citasList
            .Where(c => c.Pago?.Estado is EstadoPago.Pendiente or EstadoPago.Parcial)
            .Sum(c => c.Pago?.Saldo ?? 0);

        HistoriaClinicaDto? historiaDto = paciente.HistoriaClinica is { } h
            ? new(h.Id, h.MotivoConsulta, h.DiagnosticoInicial,
                h.DiagnosticoCie11, h.ObjetivosTerapeuticos,
                h.MedicacionActual, h.FechaIngreso, h.EstaActivo)
            : null;

        return new PacienteDetalleDto(
            paciente.Id, paciente.Nombres, paciente.Apellidos,
            paciente.NombreCompleto, paciente.Edad,
            paciente.Dni, paciente.Email, paciente.Telefono,
            paciente.TelefonoEmergencia, paciente.ContactoEmergencia,
            paciente.Genero, paciente.Ocupacion, paciente.EstadoCivil,
            paciente.Direccion, paciente.ReferidoPor, paciente.Activo,
            historiaDto, sesionesCompletadas, inasistencias, ultimaSesion, deudaPendiente);
            
    }
}
