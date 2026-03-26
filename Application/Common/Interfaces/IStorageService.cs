namespace PsychoCitas.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> SubirArchivoAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken ct = default);
    Task EliminarArchivoAsync(string url, CancellationToken ct = default);
    Task<Stream> ObtenerArchivoAsync(string url, CancellationToken ct = default);
}
