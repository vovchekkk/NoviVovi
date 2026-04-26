using System.IO.Compression;
using System.Text;
using Moq;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
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
/// Тест экспорта БОЛЬШОЙ новеллы с множеством персонажей, лейблов и выборов.
/// </summary>
public class ManualExportLargeNovelTest
{
    private readonly ITestOutputHelper _output;

    public ManualExportLargeNovelTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ExportLargeNovel_WithMultipleCharactersAndChoices_CreatesComplexZip()
    {
        // Arrange - создаем большую новеллу
        var novel = CreateLargeNovel();
        
        // Setup mocks
        var mockRepository = new Mock<INovelRepository>();
        mockRepository
            .Setup(x => x.GetByIdAsync(novel.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(novel);

        var mockStorage = new Mock<IStorageService>();
        mockStorage
            .Setup(x => x.DownloadFileStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string path, CancellationToken ct) =>
            {
                // Определяем тип изображения по пути
                var fileName = path.Contains("bg-") || path.Contains("background") 
                    ? "background.png" 
                    : "character.png";
                var imageBytes = LoadTestImage(fileName);
                return new MemoryStream(imageBytes);
            });

        var mockResourceLoader = new Mock<IEmbeddedResourceLoader>();
        mockResourceLoader
            .Setup(x => x.LoadTextResourceAsync(
                "NoviVovi.Infrastructure.Exporters.RenPy.Templates.script.rpy.sbn",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetScriptTemplate());

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

        // Сохраняем ZIP файл
        var novelTitle = novel.Title.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        var outputPath = Path.Combine(Path.GetTempPath(), $"{novelTitle}.zip");
        await File.WriteAllBytesAsync(outputPath, zipBytes);
        
        _output.WriteLine("=== ЭКСПОРТ БОЛЬШОЙ НОВЕЛЛЫ ЗАВЕРШЕН ===");
        _output.WriteLine($"ZIP файл: {outputPath}");
        _output.WriteLine($"Размер: {zipBytes.Length} байт");
        _output.WriteLine("");

        // Анализируем содержимое
        using var memoryStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        _output.WriteLine("=== СОДЕРЖИМОЕ ZIP ===");
        _output.WriteLine($"Всего файлов: {archive.Entries.Count}");
        _output.WriteLine("");

        // Читаем script.rpy
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
        
        // Проверяем, что в скрипте есть все персонажи
        Assert.Contains("Алиса", scriptContent);
        Assert.Contains("Боб", scriptContent);
        Assert.Contains("Кэт", scriptContent);
        
        // Проверяем, что есть меню выбора
        Assert.Contains("menu:", scriptContent);
        
        // Проверяем, что есть переходы между лейблами
        Assert.Contains("jump", scriptContent);
    }

    private Novel CreateLargeNovel()
    {
        var novel = Novel.Create("Большая Приключенческая Новелла");
        novel.InitializeStartLabel("start");

        // === ПЕРСОНАЖИ ===
        
        // Алиса - главная героиня
        var alice = Character.Create("Алиса", Guid.NewGuid(), Color.FromHex("#ff69b4"), "Главная героиня");
        novel.AddCharacter(alice);
        
        var aliceImage = CreateImage("alice_neutral.png", "alice-neutral-001.png");
        var aliceNeutral = CharacterState.Create("neutral", aliceImage, 
            Transform.Create(new Position(0, 0), new Size(0, 0), 1.0, 0.0, 0), "Нейтральная");
        alice.AddCharacterState(aliceNeutral);

        // Боб - друг
        var bob = Character.Create("Боб", Guid.NewGuid(), Color.FromHex("#4169e1"), "Лучший друг");
        novel.AddCharacter(bob);
        
        var bobImage = CreateImage("bob_happy.png", "bob-happy-001.png");
        var bobHappy = CharacterState.Create("happy", bobImage,
            Transform.Create(new Position(0, 0), new Size(0, 0), 1.0, 0.0, 0), "Счастливый");
        bob.AddCharacterState(bobHappy);

        // Кэт - загадочная незнакомка
        var cat = Character.Create("Кэт", Guid.NewGuid(), Color.FromHex("#9370db"), "Загадочная незнакомка");
        novel.AddCharacter(cat);
        
        var catImage = CreateImage("cat_mysterious.png", "cat-mysterious-001.png");
        var catMysterious = CharacterState.Create("mysterious", catImage,
            Transform.Create(new Position(0, 0), new Size(0, 0), 1.0, 0.0, 0), "Загадочная");
        cat.AddCharacterState(catMysterious);

        // === ФОНЫ ===
        var roomBg = CreateImage("room.png", "bg-room-001.png");
        var parkBg = CreateImage("park.png", "bg-park-001.png");
        var cafeBg = CreateImage("cafe.png", "bg-cafe-001.png");

        // === START LABEL - Пролог ===
        
        // Показываем комнату
        var roomTransform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var roomObject = BackgroundObject.Create(roomBg, roomTransform);
        var showRoomStep = ShowBackgroundStep.Create(roomObject);
        novel.StartLabel.AddStep(showRoomStep);

        // Алиса появляется
        var aliceTransform = Transform.Create(new Position(0.3, 0.1), new Size(500, 1000));
        var aliceObject = CharacterObject.Create(alice, aliceNeutral, aliceTransform);
        var showAliceStep = ShowCharacterStep.Create(aliceObject);
        novel.StartLabel.AddStep(showAliceStep);

        // Реплики
        AddReplica(novel.StartLabel, alice, "Сегодня прекрасный день для приключений!");
        AddReplica(novel.StartLabel, alice, "Интересно, что меня ждет сегодня?");

        // Боб появляется
        var bobTransform = Transform.Create(new Position(0.7, 0.1), new Size(500, 1000));
        var bobObject = CharacterObject.Create(bob, bobHappy, bobTransform);
        var showBobStep = ShowCharacterStep.Create(bobObject);
        novel.StartLabel.AddStep(showBobStep);

        AddReplica(novel.StartLabel, bob, "Привет, Алиса! Пойдем в парк?");

        // Меню выбора
        var parkLabel = Label.Create("park", novel.Id);
        var stayHomeLabel = Label.Create("stay_home", novel.Id);
        
        var choice1Transition = ChoiceTransition.Create(parkLabel);
        var choice1 = Choice.Create("Пойти в парк", choice1Transition);
        
        var choice2Transition = ChoiceTransition.Create(stayHomeLabel);
        var choice2 = Choice.Create("Остаться дома", choice2Transition);
        
        var menu = Menu.Create();
        menu.AddChoice(choice1);
        menu.AddChoice(choice2);
        
        var menuStep = ShowMenuStep.Create(menu);
        novel.StartLabel.AddStep(menuStep);

        // === PARK LABEL - Парк ===
        
        // Меняем фон на парк
        var parkTransform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var parkObject = BackgroundObject.Create(parkBg, parkTransform);
        var showParkStep = ShowBackgroundStep.Create(parkObject);
        parkLabel.AddStep(showParkStep);

        AddReplica(parkLabel, alice, "Как же здесь красиво!");
        AddReplica(parkLabel, bob, "Смотри, там кто-то стоит...");

        // Кэт появляется
        var catTransform = Transform.Create(new Position(0.5, 0.1), new Size(500, 1000));
        var catObject = CharacterObject.Create(cat, catMysterious, catTransform);
        var showCatStep = ShowCharacterStep.Create(catObject);
        parkLabel.AddStep(showCatStep);

        AddReplica(parkLabel, cat, "Привет... Я давно вас жду.");
        AddReplica(parkLabel, alice, "Кто вы?");
        AddReplica(parkLabel, cat, "Это не важно. Важно то, что я могу вам рассказать...");

        // Переход в кафе
        var cafeLabel = Label.Create("cafe", novel.Id);
        var jumpToCafe = JumpStep.Create(cafeLabel);
        parkLabel.AddStep(jumpToCafe);
        
        novel.AddLabel(parkLabel);

        // === STAY_HOME LABEL - Остаться дома ===
        
        AddReplica(stayHomeLabel, alice, "Знаешь, Боб, я лучше останусь дома.");
        AddReplica(stayHomeLabel, bob, "Как хочешь. Тогда увидимся позже!");

        // Боб уходит
        var hideBobStep = HideCharacterStep.Create(bob);
        stayHomeLabel.AddStep(hideBobStep);

        AddReplica(stayHomeLabel, alice, "Может быть, я зря отказалась?");
        AddReplica(stayHomeLabel, alice, "Ладно, почитаю книгу...");

        novel.AddLabel(stayHomeLabel);

        // === CAFE LABEL - Кафе ===
        
        // Меняем фон на кафе
        var cafeTransform = Transform.Create(new Position(0, 0), new Size(1920, 1080));
        var cafeObject = BackgroundObject.Create(cafeBg, cafeTransform);
        var showCafeStep = ShowBackgroundStep.Create(cafeObject);
        cafeLabel.AddStep(showCafeStep);

        AddReplica(cafeLabel, cat, "Присаживайтесь. Я расскажу вам одну историю...");
        AddReplica(cafeLabel, alice, "Мы слушаем.");
        AddReplica(cafeLabel, cat, "Давным-давно, в этом городе произошло нечто странное...");
        AddReplica(cafeLabel, bob, "Что именно?");
        AddReplica(cafeLabel, cat, "Но это уже совсем другая история... Продолжение следует!");

        novel.AddLabel(cafeLabel);

        return novel;
    }

    private Image CreateImage(string name, string storagePath)
    {
        var image = Image.CreatePending(
            name,
            Guid.NewGuid(),
            $"novels/images/{storagePath}",
            "png",
            ImageType.Background,
            new Size(1920, 1080)
        );
        SetImageId(image, Guid.NewGuid());
        return image;
    }

    private void AddReplica(Label label, Character character, string text)
    {
        var replica = Replica.Create(character, text);
        var replicaStep = ShowReplicaStep.Create(replica);
        label.AddStep(replicaStep);
    }

    private void SetImageId(Image image, Guid id)
    {
        var idProperty = typeof(Image).GetProperty(nameof(Image.Id))!;
        idProperty.SetValue(image, id);
    }

    private byte[] CreateFakePngBytes()
    {
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
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





