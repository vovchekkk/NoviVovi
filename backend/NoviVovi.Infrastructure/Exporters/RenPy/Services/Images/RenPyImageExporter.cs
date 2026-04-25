using System.IO.Compression;
using NoviVovi.Application.Common.Abstractions;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

/// <summary>
/// Exports images to ZIP archive with RenPy naming.
/// Follows Single Responsibility Principle: only handles image export to archive.
/// Follows Dependency Inversion Principle: depends on IStorageService abstraction.
/// </summary>
public class RenPyImageExporter(
    IStorageService storageService
) : IImageExporter
{
    public async Task ExportAsync(ZipArchive archive, IEnumerable<ImageExportInfo> images, CancellationToken ct)
    {
        foreach (var image in images)
        {
            try
            {
                await using var imageStream = await storageService.DownloadFileStreamAsync(
                    image.ImageId.ToString(), ct);
                
                var entry = archive.CreateEntry($"game/images/{image.RenPyImageName}.png");
                await using var entryStream = entry.Open();
                
                await imageStream.CopyToAsync(entryStream, bufferSize: 81920, ct);
            }
            catch (Exception ex)
            {
                // Log warning but continue export
                Console.WriteLine($"Warning: Failed to export image {image.ImageId}: {ex.Message}");
            }
        }
    }
}