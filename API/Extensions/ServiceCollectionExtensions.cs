using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PsychoCitas.Application.Common.Behaviors;
using PsychoCitas.Domain.Enums;
using PsychoCitas.Domain.Interfaces;
using PsychoCitas.Infrastructure.Persistence;
using PsychoCitas.Infrastructure.Persistence.Repositories;

namespace PsychoCitas.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        var appAssembly = typeof(PsychoCitas.Application.Features.Citas.Commands.AgendarCitaCommand).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
        services.AddValidatorsFromAssembly(appAssembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection no está configurado.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<EstadoCita>("estado_cita");
        dataSourceBuilder.MapEnum<Modalidad>("modalidad");
        dataSourceBuilder.MapEnum<TipoSesion>("tipo_sesion");

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(dataSource, b => b.MigrationsAssembly("PsychoCitas.Infrastructure")));

        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}