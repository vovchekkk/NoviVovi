using System.IO.Compression;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Orchestrates RenPy export process.
/// Follows Single Responsibility Principle: only coordinates other services.
/// Follows Dependency Inversion Principle: depends on abstractions, not concrete implementations.
/// Follows Open/Closed Principle: new export steps can be added without modifying this class.
/// </summary>
public class RenPyExporter(
    INovelRepository repository,
    NovelToRenPyMapper novelMapper,
    IRenPyScriptGenerator scriptGenerator,
    INovelImageCollector imageCollector,
    IImageExporter imageExporter,
    IRenPyArchiveBuilder archiveBuilder
) : IExporter
{
    public async Task<byte[]> ExportToRenPyAsync(Guid novelId, CancellationToken ct)
    {
        // 1. Load novel from repository
        var novel = await repository.GetByIdAsync(novelId, ct)
                    ?? throw new NotFoundException($"Novel '{novelId}' not found");

        // 2. Map Domain Novel to RenPy models
        var renPyNovel = novelMapper.Map(novel);

        // 3. Build ZIP archive
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // ВАЖНО: Порядок добавления файлов!
            // 1. Сначала добавляем НАШИ файлы (script.rpy и images)
            // 2. Потом добавляем BaseProject (пропуская уже существующие)
            
            // Generate and add script.rpy
            var scriptContent = await scriptGenerator.GenerateAsync(renPyNovel, ct);
            archiveBuilder.AddTextFile(archive, "game/script.rpy", scriptContent);

            // Collect and export images
            var images = imageCollector.CollectImages(novel);
            await imageExporter.ExportAsync(archive, images, ct);

            // Add base project files (will skip existing entries)
            await archiveBuilder.AddBaseProjectFilesAsync(archive, ct);
        }

        return memoryStream.ToArray();
    }
}