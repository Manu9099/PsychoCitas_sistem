namespace PsychoCitas.Domain.Entities;

public class Usuario : BaseEntity
{
    public string NombreUsuario { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Rol { get; private set; } = "Admin";
    public bool Activo { get; private set; } = true;

    protected Usuario() { }

    public static Usuario Crear(string nombreUsuario, string email, string passwordHash, string rol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreUsuario);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(rol);

        return new Usuario
        {
            NombreUsuario = nombreUsuario.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Rol = rol.Trim()
        };
    }

    public void CambiarPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void Desactivar() => Activo = false;
}