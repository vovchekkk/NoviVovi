using System.IO.Compression;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

/// <summary>
/// Exports images to ZIP archive with RenPy naming.
/// Single Responsibility: Image export to archive.
/// </summary>
public interface IImageExporter
{
    Task ExportAsync(ZipArchive archive, IEnumerable<ImageExportInfo> images, CancellationToken ct);
}
