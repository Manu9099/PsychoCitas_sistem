using MediatR;
using PsychoCitas.Application.DTOs;
using PsychoCitas.Domain.Interfaces;

namespace PsychoCitas.Application.Features.Pacientes.Queries;

public record BuscarPacientesQuery(string Termino) : IRequest<IEnumerable<PacienteDto>>;

public class BuscarPacientesHandler(IUnitOfWork uow) : IRequestHandler<BuscarPacientesQuery, IEnumerable<PacienteDto>>
{
    public async Task<IEnumerable<PacienteDto>> Handle(BuscarPacientesQuery query, CancellationToken ct)
    {
        var pacientes = await uow.Pacientes.BuscarAsync(query.Termino, ct);
        return pacientes.Select(p => new PacienteDto(
            p.Id, p.Nombres, p.Apellidos, p.NombreCompleto,
            p.Edad, p.Dni, p.Email, p.Telefono,
            p.Genero, p.Ocupacion, p.ReferidoPor, p.Activo, p.CreadoEn));
    }
}
