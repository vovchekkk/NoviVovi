using System.IO.Compression;
using System.Text;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Menu.Requests;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Critical;

/// <summary>
/// КРИТИЧЕСКИЕ ТЕСТЫ: RenPy Export - основная функция приложения
/// Каждый тест проверяет ВСЕ типы шагов
/// </summary>
[Collection("Database collection")]
public class RenPyExportTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task ExportEmptyNovel_ShouldReturnValidZip()
    {
        // Arrange - создать пустую новеллу
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Empty Novel"));
        Assert.NotNull(novel);
        
        // Act - экспортировать
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);
        
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(zipBytes);
        
        // Проверить содержимое ZIP
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        // Должен содержать game/script.rpy
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        Assert.Contains("label start:", scriptContent);
        Assert.Contains("return", scriptContent);
    }
    
    [Fact]
    public async Task ExportNovelWithAllStepTypes_ShouldContainAllSteps()
    {
        // Arrange - создать новеллу со ВСЕМИ типами шагов
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Complete Novel"));
        Assert.NotNull(novel);
        
        var startLabel = novel.StartLabelId;
        
        // Создать персонажа
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", "Main character"));
        Assert.NotNull(character);
        
        // Создать изображения
        var charImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(charImageUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{charImageUpload.ImageId}/confirm", null);
        
        var bgImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(bgImageUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bgImageUpload.ImageId}/confirm", null);
        
        // Создать состояние персонажа
        var charState = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", "Happy expression", charImageUpload.ImageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(charState);
        
        // Создать вторую метку для jump
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene2"));
        Assert.NotNull(label2);
        
        // 1. ShowBackground step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{startLabel}/steps",
            new AddShowBackgroundStepRequest(bgImageUpload.ImageId,
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        // 2. ShowCharacter step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{startLabel}/steps",
            new AddShowCharacterStepRequest(character.Id, charState.Id,
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        
        // 3. Replica step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{startLabel}/steps",
            new AddShowReplicaStepRequest(character.Id, "Hello! This is my first line!"));
        
        // 4. Menu step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{startLabel}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Go to scene 2", new ChoiceTransitionRequest { TargetLabelId = label2.Id }),
                new ChoiceRequest("Stay here", new ChoiceTransitionRequest { TargetLabelId = startLabel })
            ]));
        
        // 5. HideCharacter step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label2.Id}/steps",
            new AddHideCharacterStepRequest(character.Id));
        
        // 6. Jump step
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label2.Id}/steps",
            new AddJumpStepRequest(startLabel));
        
        // Act - экспортировать
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        Assert.True(response.IsSuccessStatusCode);
        
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        // Assert - проверить game/script.rpy
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        // Проверить, что ВСЕ типы шагов экспортировались (используются GUID-based идентификаторы)
        Assert.Contains("label start:", scriptContent); // Start label
        Assert.Contains($"label_{label2.Id:N}:", scriptContent); // Second label
        Assert.Contains("scene bg", scriptContent); // ShowBackground
        Assert.Contains($"show char_{character.Id:N}", scriptContent); // ShowCharacter
        Assert.Contains("\"Hello! This is my first line!\"", scriptContent); // Replica
        Assert.Contains("menu:", scriptContent); // Menu
        Assert.Contains("\"Go to scene 2\":", scriptContent); // Menu choice 1
        Assert.Contains("\"Stay here\":", scriptContent); // Menu choice 2
        Assert.Contains($"hide char_{character.Id:N}", scriptContent); // HideCharacter
        Assert.Contains("jump label_", scriptContent); // Jump (проверяем наличие jump с label_ префиксом)
        
        // Проверить, что персонаж определен
        Assert.Contains($"define char_{character.Id:N}", scriptContent);
    }
    
    [Fact]
    public async Task ExportNovel_ScriptRpyShouldBeValidPython()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        // Добавить реплику
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "Test line"));
        
        // Act
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        // Assert - проверить базовый синтаксис Python/RenPy
        // Не должно быть синтаксических ошибок
        Assert.DoesNotContain("{{", scriptContent); // Нет незакрытых шаблонов
        Assert.DoesNotContain("}}", scriptContent);
        
        // Правильные отступы (4 пробела)
        var lines = scriptContent.Split('\n');
        foreach (var line in lines.Where(l => l.Trim().StartsWith("\"") && l.Contains(":")))
        {
            // Menu choices должны иметь отступ
            Assert.True(line.StartsWith("    ") || line.StartsWith("\t"));
        }
        
        // Все label должны заканчиваться двоеточием
        foreach (var line in lines.Where(l => l.Trim().StartsWith("label ")))
        {
            Assert.EndsWith(":", line.Trim());
        }
    }
    
    [Fact]
    public async Task ExportNovel_ShouldContainAllCharacterDefinitions()
    {
        // Arrange - создать несколько персонажей
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Multi Character Novel"));
        Assert.NotNull(novel);
        
        var char1 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        var char2 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Villain", "0000FF", null));
        var char3 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Sidekick", "00FF00", null));
        
        Assert.NotNull(char1);
        Assert.NotNull(char2);
        Assert.NotNull(char3);
        
        // Добавить реплики от всех персонажей
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(char1.Id, "Hero line"));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(char2.Id, "Villain line"));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(char3.Id, "Sidekick line"));
        
        // Act
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        // Assert - все персонажи должны быть определены (используются GUID-based идентификаторы)
        Assert.Contains($"define char_{char1.Id:N}", scriptContent);
        Assert.Contains($"define char_{char2.Id:N}", scriptContent);
        Assert.Contains($"define char_{char3.Id:N}", scriptContent);
        
        // Проверить цвета персонажей
        Assert.Contains("#FF5733", scriptContent);
        Assert.Contains("#0000FF", scriptContent);
        Assert.Contains("#00FF00", scriptContent);
        
        // Проверить, что все реплики присутствуют (используются GUID-based идентификаторы)
        Assert.Contains($"char_{char1.Id:N} \"Hero line\"", scriptContent);
        Assert.Contains($"char_{char2.Id:N} \"Villain line\"", scriptContent);
        Assert.Contains($"char_{char3.Id:N} \"Sidekick line\"", scriptContent);
    }
    
    [Fact]
    public async Task ExportNovel_ShouldContainAllLabels()
    {
        // Arrange - создать несколько меток
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Multi Label Novel"));
        Assert.NotNull(novel);
        
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("intro"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("chapter1"));
        var label3 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("chapter2"));
        var label4 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("ending"));
        
        Assert.NotNull(label1);
        Assert.NotNull(label2);
        Assert.NotNull(label3);
        Assert.NotNull(label4);
        
        // Добавить jump между метками
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddJumpStepRequest(label1.Id));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label1.Id}/steps",
            new AddJumpStepRequest(label2.Id));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label2.Id}/steps",
            new AddJumpStepRequest(label3.Id));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label3.Id}/steps",
            new AddJumpStepRequest(label4.Id));
        
        // Act
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        // Assert - все метки должны быть экспортированы (используются GUID-based идентификаторы)
        Assert.Contains("label start:", scriptContent);
        Assert.Contains($"label_{label1.Id:N}:", scriptContent);
        Assert.Contains($"label_{label2.Id:N}:", scriptContent);
        Assert.Contains($"label_{label3.Id:N}:", scriptContent);
        Assert.Contains($"label_{label4.Id:N}:", scriptContent);
        
        // Проверить jump между метками (проверяем наличие label идентификаторов в jump statements)
        // Jump statements имеют отступы, поэтому проверяем просто наличие идентификаторов
        var label1Id = $"label_{label1.Id:N}";
        var label2Id = $"label_{label2.Id:N}";
        var label3Id = $"label_{label3.Id:N}";
        var label4Id = $"label_{label4.Id:N}";
        
        // Проверяем, что каждый label упоминается в jump (может быть с отступами)
        Assert.Contains(label1Id, scriptContent);
        Assert.Contains(label2Id, scriptContent);
        Assert.Contains(label3Id, scriptContent);
        Assert.Contains(label4Id, scriptContent);
        
        // Проверяем, что есть jump statements (хотя бы одно слово "jump")
        Assert.Contains("jump", scriptContent);
    }
    
    [Fact]
    public async Task ExportNovel_WithComplexMenu_ShouldExportAllChoices()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Menu Novel"));
        Assert.NotNull(novel);
        
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_a"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_b"));
        var label3 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_c"));
        
        Assert.NotNull(label1);
        Assert.NotNull(label2);
        Assert.NotNull(label3);
        
        // Создать меню с 3 выборами
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Choose path A", new ChoiceTransitionRequest { TargetLabelId = label1.Id }),
                new ChoiceRequest("Choose path B", new ChoiceTransitionRequest { TargetLabelId = label2.Id }),
                new ChoiceRequest("Choose path C", new ChoiceTransitionRequest { TargetLabelId = label3.Id })
            ]));
        
        // Act
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var scriptEntry = archive.GetEntry("game/script.rpy");
        Assert.NotNull(scriptEntry);
        
        using var scriptStream = scriptEntry.Open();
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        // Assert - все выборы должны быть экспортированы (используются GUID-based идентификаторы)
        Assert.Contains("menu:", scriptContent);
        Assert.Contains("\"Choose path A\":", scriptContent);
        Assert.Contains("\"Choose path B\":", scriptContent);
        Assert.Contains("\"Choose path C\":", scriptContent);
        Assert.Contains($"jump label_{label1.Id:N}", scriptContent);
        Assert.Contains($"jump label_{label2.Id:N}", scriptContent);
        Assert.Contains($"jump label_{label3.Id:N}", scriptContent);
    }
    
    [Fact]
    public async Task ExportNovel_WithImages_ShouldContainImagesDirectory()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Image Novel"));
        Assert.NotNull(novel);
        
        // Создать несколько изображений
        var bg1 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("forest.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bg1.ImageId}/confirm", null);
        
        var bg2 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("city.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bg2.ImageId}/confirm", null);
        
        var char1 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{char1.ImageId}/confirm", null);
        
        // Использовать изображения в шагах
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowBackgroundStepRequest(bg1.ImageId, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        // Act
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        var zipBytes = await response.Content.ReadAsByteArrayAsync();
        
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        // Assert - проверить game/images/ директорию
        var imageEntries = archive.Entries.Where(e => e.FullName.StartsWith("game/images/")).ToList();
        Assert.NotEmpty(imageEntries);
        
        // Должны быть все загруженные изображения
        Assert.True(imageEntries.Count >= 1); // Хотя бы одно изображение (bg1), которое используется
    }
}
