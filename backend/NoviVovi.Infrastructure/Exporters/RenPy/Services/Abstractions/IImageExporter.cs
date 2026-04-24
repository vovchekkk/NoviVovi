using System.IO.Compression;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

/// <summary>
/// Exports images to ZIP archive.
/// Single Responsibility: Image export to archive.
/// </summary>
public interface IImageExporter
{
    Task ExportAsync(ZipArchive archive, IEnumerable<Guid> imageIds, CancellationToken ct);
}
