using Microsoft.Extensions.Options;
using PsychoCitas.Application.Common.Interfaces;
using PsychoCitas.Infrastructure.Options;

namespace PsychoCitas.Infrastructure.Services.Storage;

public class LocalStorageService(IOptions<LocalStorageOptions> options) : IStorageService
{
    private readonly LocalStorageOptions _options = options.Value;

    public async Task<string> SubirArchivoAsync(
        Stream contenido,
        string nombreArchivo,
        string contentType,
        CancellationToken ct = default)
    {
        var root = Path.GetFullPath(_options.BasePath);
        Directory.CreateDirectory(root);

        var filePath = Path.Combine(root, nombreArchivo);

        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await contenido.CopyToAsync(fs, ct);

        return filePath;
    }

    public Task EliminarArchivoAsync(string url, CancellationToken ct = default)
    {
        if (File.Exists(url))
            File.Delete(url);

        return Task.CompletedTask;
    }

    public Task<Stream> ObtenerArchivoAsync(string url, CancellationToken ct = default)
    {
        Stream stream = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }
}