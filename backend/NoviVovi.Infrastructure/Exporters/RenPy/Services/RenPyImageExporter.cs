using System.IO.Compression;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Exports images to ZIP archive.
/// Follows Single Responsibility Principle: only handles image export to archive.
/// Follows Dependency Inversion Principle: depends on IStorageService abstraction.
/// </summary>
public class RenPyImageExporter(
    IStorageService storageService
) : IImageExporter
{
    public async Task ExportAsync(ZipArchive archive, IEnumerable<Guid> imageIds, CancellationToken ct)
    {
        foreach (var imageId in imageIds)
        {
            try
            {
                var imageBytes = await storageService.DownloadFileAsync(imageId.ToString(), ct);
                var entry = archive.CreateEntry($"game/images/{imageId:N}.png");

                await using var entryStream = entry.Open();
                await entryStream.WriteAsync(imageBytes, ct);
            }
            catch (Exception ex)
            {
                // Log warning but continue export
                Console.WriteLine($"Warning: Failed to export image {imageId}: {ex.Message}");
            }
        }
    }
}