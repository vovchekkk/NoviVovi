using System.IO.Compression;
using System.Text;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Export.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Models;
using Scriban;
using ImageType = NoviVovi.Infrastructure.DatabaseObjects.Enums.ImageType;

// TODO: Временно закомментировано - будет переделано с новой архитектурой
/*
public class RenPyExporter(
    INovelRepository repository,
    IStorageService storageService,
    RenPyIdentifierGenerator idGenerator
) : IExporter
{
    public async Task<byte[]> ExportToRenPyAsync(Guid novelId, CancellationToken ct)
    {
        var novel = await repository.GetByIdAsync(novelId, ct)
                    ?? throw new NotFoundException($"Novel '{novelId}' not found");

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // 1. Генерируем script.rpy
            var scriptContent = await GenerateScriptRpy(novel);
            AddTextFile(archive, "game/script.rpy", scriptContent);

            // 2. Экспортируем изображения
            await ExportImages(archive, novel, ct);

            // 3. Добавляем статические файлы (options.rpy, screens.rpy)
            // TODO: добавить шаблоны
        }

        return memoryStream.ToArray();
    }
    
    private async Task<string> GenerateScriptRpy(Novel novel)
    {
        // Здесь используем Scriban. Подготавливаем анонимный объект для шаблона
        var model = new {
            Characters = novel.Characters.Select(c => new { 
                VarName = c.Name.ToRenPyId(), 
                Name = c.Name 
            }),
            Labels = novel.Labels.Select(l => new {
                Id = l.Name.ToRenPyId(),
                Steps = l.Steps.Select(MapStepToCode)
            })
        };

        var template = Template.Parse("...твой шаблон...");
        return await template.RenderAsync(model);
    }

    private string MapStepToCode(Step step, int indentLevel = 1)
    {
        // Превращаем каждый шаг в строку кода Ren'Py (Python)
        // Не забывай про отступы в 4 пробела!
        var indent = new string(' ', indentLevel * 4);
        
        return step switch {
            HideCharacterStep s => $"    hide {s.Character.Name.ToRenPyId()}",
            JumpStep s => $"    jump {s.Transition.TargetLabel.Name.ToRenPyId()}",
            ShowBackgroundStep s => $"    scene {s.BackgroundObject.Image.Name.ToRenPyId()}",
            ShowCharacterStep s => $"    show {s.CharacterObject.Character.Name.ToRenPyId()} {s.CharacterObject.State.Name.ToRenPyId()} at {s.CharacterObject.Transform.ToRenPyPosition()}",
            ShowMenuStep s => MapMenu(s.Menu, indentLevel),
            ShowReplicaStep s => $"    {s.Replica.Speaker.Name.ToRenPyId()} \"{s.Replica.Text}\"",
            _ => "    pass"
        };
    }
    
    private string MapMenu(Menu menu, int indentLevel)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', indentLevel * 4);
        var childIndent = new string(' ', (indentLevel + 1) * 4);
        var stepIndent = new string(' ', (indentLevel + 2) * 4);

        sb.AppendLine($"{indent}menu:");

        foreach (var choice in menu.Choices)
        {
            // 1. Текст выбора
            sb.AppendLine($"{childIndent}\"{choice.Text}\":");

            // 2. Действие после выбора (обычно это переход)
            // В твоем домене Choice имеет Transition, который ведет к Label
            var targetId = choice.Transition.TargetLabel.Name.ToRenPyId();
            sb.AppendLine($"{stepIndent}jump {targetId}");
        }

        return sb.ToString().TrimEnd(); // Убираем последний перенос строки для красоты
    }
    private async Task ExportImages(ZipArchive archive, Novel novel, CancellationToken ct)
    {
        // Собираем все уникальные изображения из новеллы
        var imageReferences = CollectImageReferences(novel);

        foreach (var imageRef in imageReferences)
        {
            // Получаем байты из storage
            var imageBytes = await storageService.GetFileAsync(imageRef.StoragePath, ct);

            // Генерируем имя файла для Ren'Py
            var renPyFileName = GenerateRenPyImageFileName(imageRef);

            // Добавляем в ZIP: game/images/{filename}
            var entry = archive.CreateEntry($"game/images/{renPyFileName}");
            using var entryStream = await entry.OpenAsync(ct);
            await entryStream.WriteAsync(imageBytes, 0, imageBytes.Length, ct);
        }
    }

    private string GenerateRenPyImageFileName(ImageReference imageRef)
    {
        // Используем GUID для уникальности + оригинальное расширение
        var extension = Path.GetExtension(imageRef.OriginalFileName); // .png, .jpg

        return imageRef.Type switch
        {
            ImageType.Background => $"bg_{imageRef.Id:N}{extension}",
            ImageType.Character => $"char_{imageRef.CharacterId:N}_{imageRef.StateName}_{imageRef.Id:N}{extension}",
            _ => $"img_{imageRef.Id:N}{extension}"
        };
    }

    private List<ImageReference> CollectImageReferences(Novel novel)
    {
        var images = new List<ImageReference>();

        // Собираем все фоны
        foreach (var label in novel.Labels)
        {
            foreach (var step in label.Steps)
            {
                if (step is ShowBackgroundStep bgStep)
                {
                    images.Add(new ImageReference
                    {
                        Id = bgStep.BackgroundObject.Image.Id,
                        StoragePath = bgStep.BackgroundObject.Image.StoragePath,
                        OriginalFileName = bgStep.BackgroundObject.Image.FileName,
                        Type = ImageType.Background
                    });
                }

                if (step is ShowCharacterStep charStep)
                {
                    images.Add(new ImageReference
                    {
                        Id = charStep.CharacterObject.State.Image.Id,
                        CharacterId = charStep.CharacterObject.Character.Id,
                        StateName = charStep.CharacterObject.State.Name,
                        StoragePath = charStep.CharacterObject.State.Image.StoragePath,
                        OriginalFileName = charStep.CharacterObject.State.Image.FileName,
                        Type = ImageType.Character
                    });
                }
            }
        }

        // Убираем дубликаты
        return images.DistinctBy(i => i.Id).ToList();
    }

    private void AddTextFile(ZipArchive archive, string path, string content)
    {
        var entry = archive.CreateEntry(path);
        using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.Write(content);
    }
}
*/