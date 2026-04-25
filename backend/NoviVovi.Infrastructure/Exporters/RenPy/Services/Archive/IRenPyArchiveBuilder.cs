using System.IO.Compression;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;

/// <summary>
/// Builds RenPy project ZIP archive.
/// Single Responsibility: Archive construction.
/// </summary>
public interface IRenPyArchiveBuilder
{
    void AddTextFile(ZipArchive archive, string path, string content);
    Task AddBaseProjectFilesAsync(ZipArchive archive, CancellationToken ct);
}
