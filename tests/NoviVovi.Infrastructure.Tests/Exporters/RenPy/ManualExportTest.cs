using System.IO.Compression;
using System.Text;
using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Resources;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy;

/// <summary>
/// Ручной тест для проверки реального экспорта микро-новеллы.
/// Создает ZIP файл, который можно открыть и проверить.
/// </summary>
public class ManualExportTest
{
    private readonly ITestOutputHelper _output;

    public ManualExportTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ExportMicroNovel_CreatesRealZipFile()
    {
        // Arrange - создаем микро-новеллу
        var novel = CreateMicroNovel();
        
        // Setup mocks
        var mockRepository = new Mock<INovelRepository>();
        mockRepository
            .Setup(x => x.GetByIdAsync(novel.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        var mockStorage = new Mock<IStorageService>();
        // Мокаем загрузку изображений - возвращаем реальные или fake PNG
        mockStorage
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string path, CancellationToken ct) =>
            {
                // Определяем тип изображения по пути
                var fileName = path.Contains("bg-room") ? "background.png" : "character.png";
                var imageBytes = LoadTestImage(fileName);
                return new MemoryStream(imageBytes);
            });

        var mockResourceLoader = new Mock<IEmbeddedResourceLoader>();
        mockResourceLoader
            .Setup(x => x.LoadTextResourceAsync(
                "NoviVovi.Infrastructure.Exporters.RenPy.Templates.script.rpy.sbn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetScriptTemplate());

        // ИСПРАВЛЕНО: Используем реальный EmbeddedResourceLoader для BaseProject.zip
        var realResourceLoader = new EmbeddedResourceLoader();
        mockResourceLoader
            .Setup(x => x.LoadStreamResourceAsync(
                "NoviVovi.Infrastructure.Exporters.RenPy.Resources.BaseProject.zip",
                It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((name, ct) => 
                realResourceLoader.LoadStreamResourceAsync(name, ct));

        // Setup real services
        var identifierGenerator = new RenPyIdentifierGenerator();
        var transformMapper = new TransformToRenPyMapper();
        var characterMapper = new CharacterToRenPyMapper(identifierGenerator);
        var stepMapper = new StepToRenPyMapper(identifierGenerator, transformMapper);
        var labelMapper = new LabelToRenPyMapper(identifierGenerator, stepMapper);
        var novelMapper = new NovelToRenPyMapper(identifierGenerator, characterMapper, labelMapper);
        
        var statementRenderer = new RenPyStatementRenderer();
        var scriptGenerator = new RenPyScriptGenerator(mockResourceLoader.Object, statementRenderer);
        
        var imageCollector = new NovelImageCollector();
        var imageExporter = new RenPyImageExporter(mockStorage.Object);
        
        var archiveBuilder = new RenPyArchiveBuilder(mockResourceLoader.Object);

        var exporter = new RenPyExporter(
            mockRepository.Object,
            novelMapper,
            scriptGenerator,
            imageCollector,
            imageExporter,
            archiveBuilder
        );

        // Act - экспортируем
        var zipBytes = await exporter.ExportToRenPyAsync(novel.Id, CancellationToken.None);

        // Assert - проверяем и сохраняем
        Assert.NotNull(zipBytes);
        Assert.NotEmpty(zipBytes);

        // Сохраняем ZIP файл для ручной проверки
        var novelTitle = novel.Title.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        var outputPath = Path.Combine(Path.GetTempPath(), $"{novelTitle}.zip");
        await File.WriteAllBytesAsync(outputPath, zipBytes);
        
        _output.WriteLine("=== ЭКСПОРТ ЗАВЕРШЕН ===");
        _output.WriteLine($"ZIP файл сохранен: {outputPath}");
        _output.WriteLine($"Размер: {zipBytes.Length} байт");
        _output.WriteLine("");

        // Анализируем содержимое ZIP
        using var memoryStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        _output.WriteLine("=== СОДЕРЖИМОЕ ZIP ===");
        foreach (var entry in archive.Entries.OrderBy(e => e.FullName))
        {
            _output.WriteLine($"  {entry.FullName} ({entry.Length} байт)");
        }
        _output.WriteLine("");

        // Читаем и выводим script.rpy
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);

        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream, Encoding.UTF8);
        var scriptContent = await reader.ReadToEndAsync();

        _output.WriteLine("=== СОДЕРЖИМОЕ game/script.rpy ===");
        _output.WriteLine(scriptContent);
        _output.WriteLine("");

        // Проверяем изображения
        var imageEntries = archive.Entries.Where(e => e.FullName.StartsWith("game/images/")).ToList();
        _output.WriteLine($"=== ИЗОБРАЖЕНИЯ ({imageEntries.Count}) ===");
        foreach (var img in imageEntries)
        {
            _output.WriteLine($"  {img.FullName}");
        }

        _output.WriteLine("");
        _output.WriteLine("✅ Тест завершен! Проверьте ZIP файл вручную.");
    }

    private Novel CreateMicroNovel()
    {
        // Создаем новеллу
        var novel = Novel.Create("Моя Первая Новелла");       var startLabel = novel.InitializeStartLabel("start");

        // Создаем персонажа
        var alice = Character.Create("Алиса", Guid.NewGuid(), Color.FromHex("#ff69b4"), "Главная героиня");
        novel.AddCharacter(alice);

        // Создаем изображение для фона
        var bgImage = Image.CreatePending("room.png", Guid.NewGuid(), "novels/images/bg-room-001.png", "png", ImageType.Background,
            new Size(1920, 1080)
        );
        SetImageId(bgImage, Guid.Parse("11111111-2222-3333-4444-555555555555"));

        // Создаем изображение для персонажа (РЕАЛЬНЫЙ размер твоего character.png)
        var aliceImage = Image.CreatePending("alice_happy.png", Guid.NewGuid(), "novels/images/alice-happy-001.png", "png", ImageType.Character,
            new Size(250, 500) // ← Измени на РЕАЛЬНЫЙ размер твоего изображения!
        );
        SetImageId(aliceImage, Guid.Parse("66666666-7777-8888-9999-000000000000"));

        // Создаем состояние персонажа (с нулевым LocalTransform чтобы избежать бага с оператором +)
        var aliceHappy = CharacterState.Create(
            "happy",
            aliceImage,
            Transform.Create(new Position(0, 0), new Size(0, 0), 1.0, 0.0, 0), // LocalTransform = НУЛЕВОЙ размер!
            "Счастливая Алиса"
        );
        alice.AddCharacterState(aliceHappy);

        // Добавляем шаги в start label
        
        // 1. Показываем фон
        var bgTransform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var bgObject = BackgroundObject.Create(bgImage, bgTransform);
        var showBgStep = ShowBackgroundStep.Create(bgObject);
        novel.StartLabel.AddStep(showBgStep);

        // 2. Показываем персонажа (относительные координаты, внизу экрана)
        var aliceTransform = Transform.Create(new Position(0.5, 1.0), new Size(250, 500));
        var aliceObject = CharacterObject.Create(alice, aliceHappy, aliceTransform);
        var showAliceStep = ShowCharacterStep.Create(aliceObject);
        novel.StartLabel.AddStep(showAliceStep);

        // 3. Реплика
        var replica1 = Replica.Create(alice, "Привет! Это моя первая новелла на RenPy!");
        var replicaStep1 = ShowReplicaStep.Create(replica1);
        novel.StartLabel.AddStep(replicaStep1);

        // 4. Еще реплика
        var replica2 = Replica.Create(alice, "Надеюсь, экспорт работает правильно! 😊");
        var replicaStep2 = ShowReplicaStep.Create(replica2);
        novel.StartLabel.AddStep(replicaStep2);

        return novel;
    }

    private void SetImageId(Image image, Guid id)
    {
        var idProperty = typeof(Image).GetProperty(nameof(Image.Id))!;
        idProperty.SetValue(image, id);
    }

    private byte[] CreateFakePngBytes()
    {
        // Минимальный валидный PNG (1x1 прозрачный пиксель)
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, // IHDR chunk
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41,
            0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
            0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
            0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
            0x42, 0x60, 0x82
        };
    }

    /// <summary>
    /// Загружает реальное изображение из TestData/Images или возвращает fake PNG.
    /// Поддерживает форматы: .png, .jpg, .jpeg
    /// </summary>
    private byte[] LoadTestImage(string fileName)
    {
        // Ищем папку TestData относительно сборки
        var baseDir = AppContext.BaseDirectory; // bin/Debug/net10.0/
        var imagesDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "TestData", "Images"));
        
        // Пробуем разные расширения
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        var extensions = new[] { ".png", ".jpg", ".jpeg" };
        
        foreach (var ext in extensions)
        {
            var testDataPath = Path.Combine(imagesDir, baseName + ext);
            if (File.Exists(testDataPath))
            {
                _output.WriteLine($"✅ Используется реальное изображение: {testDataPath}");
                return File.ReadAllBytes(testDataPath);
            }
        }

        _output.WriteLine($"⚠️ Файл {fileName} не найден (искали .png, .jpg, .jpeg), используется fake PNG");
        _output.WriteLine($"   Положите реальное изображение в: {imagesDir}");
        return CreateFakePngBytes();
    }

    private string GetScriptTemplate()
    {
        return @"## Generated by NoviVovi - {{ title }}

## Character Definitions
{{~ for character in characters ~}}
define {{ character.variable_name }} = Character(""{{ character.display_name }}"", color=""{{ character.color }}"")
{{~ end ~}}

## Game Start
label start:
{{~ for label in labels ~}}
{{~ if label.identifier == start_label_id ~}}
{{~ for statement in label.statements ~}}
{{ render_statement statement }}
{{~ end ~}}
{{~ end ~}}
{{~ end ~}}
    return

## Labels
{{~ for label in labels ~}}
{{~ if label.identifier != start_label_id ~}}

label {{ label.identifier }}:
{{~ for statement in label.statements ~}}
{{ render_statement statement }}
{{~ end ~}}
    return
{{~ end ~}}
{{~ end ~}}";
    }
}


