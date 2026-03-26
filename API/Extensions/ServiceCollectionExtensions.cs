using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoCitas.Application.Common.Behaviors;
using PsychoCitas.Domain.Interfaces;
using PsychoCitas.Infrastructure.Persistence;
using PsychoCitas.Infrastructure.Persistence.Repositories;

namespace PsychoCitas.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // MediatR + FluentValidation
        var appAssembly = typeof(PsychoCitas.Application.Features.Citas.Commands.AgendarCitaCommand).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
        services.AddValidatorsFromAssembly(appAssembly);

        // Pipeline behaviors (orden importa)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // EF Core + PostgreSQL
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("PsychoCitas.Infrastructure")));

        // Repositories & UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
