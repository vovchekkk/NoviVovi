using System.IO.Compression;
using System.Text;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Resources;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;

/// <summary>
/// Builds RenPy project ZIP archive.
/// Follows Single Responsibility Principle: only handles archive construction.
/// </summary>
public class RenPyArchiveBuilder(
    IEmbeddedResourceLoader resourceLoader
) : IRenPyArchiveBuilder
{
    private const string BaseProjectResourceName = "NoviVovi.Infrastructure.Exporters.RenPy.Resources.BaseProject.zip";

    public void AddTextFile(ZipArchive archive, string path, string content)
    {
        var entry = archive.CreateEntry(path);
        using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.Write(content);
    }

    public async Task AddBaseProjectFilesAsync(ZipArchive archive, CancellationToken ct)
    {
        await using var stream = await resourceLoader.LoadStreamResourceAsync(BaseProjectResourceName, ct);
        if (stream == null)
        {
            // BaseProject.zip is optional
            return;
        }

        using var baseArchive = new ZipArchive(stream, ZipArchiveMode.Read);
        
        foreach (var entry in baseArchive.Entries)
        {
            // Убираем префикс "BaseProject/" из пути
            var entryPath = entry.FullName;
            if (entryPath.StartsWith("BaseProject/"))
            {
                entryPath = entryPath.Substring("BaseProject/".Length);
            }
            
            // Пропускаем пустые записи (папки)
            if (string.IsNullOrEmpty(entryPath))
                continue;
            
            // Просто копируем все файлы из BaseProject.zip
            // BaseProject.zip НЕ содержит game/script.rpy и game/images/*
            var newEntry = archive.CreateEntry(entryPath);
            await using var sourceStream = entry.Open();
            await using var destStream = newEntry.Open();
            await sourceStream.CopyToAsync(destStream, ct);
        }
    }
}