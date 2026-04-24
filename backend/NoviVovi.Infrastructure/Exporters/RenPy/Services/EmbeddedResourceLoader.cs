using System.Reflection;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Loads embedded resources from assembly.
/// Follows Single Responsibility Principle: only handles resource loading.
/// </summary>
public class EmbeddedResourceLoader : IEmbeddedResourceLoader
{
    private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public async Task<string> LoadTextResourceAsync(string resourceName, CancellationToken ct = default)
    {
        await using var stream = await LoadStreamResourceAsync(resourceName, ct);
        if (stream == null)
            throw new InvalidOperationException($"Resource '{resourceName}' not found in embedded resources");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(ct);
    }

    public async Task<Stream?> LoadStreamResourceAsync(string resourceName, CancellationToken ct = default)
    {
        var stream = _assembly.GetManifestResourceStream(resourceName);
        return await Task.FromResult(stream);
    }
}
