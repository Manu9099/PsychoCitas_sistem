using Microsoft.EntityFrameworkCore;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Domain.Entities;
using PsychoCitas.Infrastructure.Persistence;

namespace PsychoCitas.API.Extensions;

public static class SeedExtensions
{
    public static async Task SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await db.Database.MigrateAsync();

        if (await db.Usuarios.AnyAsync())
            return;

        var admin = Usuario.Crear(
            "admin",
            "admin@psychocitas.com",
            hasher.Hash("Admin123*"),
            "Admin"
        );

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();
    }
}