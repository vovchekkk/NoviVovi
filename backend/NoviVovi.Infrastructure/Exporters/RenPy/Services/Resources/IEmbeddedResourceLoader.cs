namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Resources;

/// <summary>
/// Loads embedded resources from assembly.
/// Single Responsibility: Resource loading.
/// </summary>
public interface IEmbeddedResourceLoader
{
    Task<string> LoadTextResourceAsync(string resourceName, CancellationToken ct = default);
    Task<Stream?> LoadStreamResourceAsync(string resourceName, CancellationToken ct = default);
}
