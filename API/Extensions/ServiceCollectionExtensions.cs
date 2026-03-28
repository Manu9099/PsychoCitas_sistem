using PsychoCitas.Domain.Enums;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using PsychoCitas.Infrastructure.Persistence;
using PsychoCitas.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using PsychoCitas.Infrastructure.Identity;
using PsychoCitas.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using PsychoCitas.Application.Common.Behaviors;


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
        dataSourceBuilder.MapEnum<EstadoCita>();
        dataSourceBuilder.MapEnum<Modalidad>();
        dataSourceBuilder.MapEnum<TipoSesion>();

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(dataSource, b => b.MigrationsAssembly("PsychoCitas.Infrastructure")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
}