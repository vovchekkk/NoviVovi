using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Menu.Requests;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Critical;

/// <summary>
/// КРИТИЧЕСКИЕ ТЕСТЫ: Preview - используется фронтендом для отображения сцены
/// Каждый тест проверяет ВСЕ типы шагов и их комбинации
/// </summary>
[Collection("Database collection")]
public class PreviewExtendedTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Preview_StepWithoutBackground_ShouldReturnNullBackground()
    {
        // Arrange - создать шаг без фона
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var replicaStep = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "Test line"));
        Assert.NotNull(replicaStep);
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{replicaStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.Null(preview.Background); // Нет фона
        Assert.NotNull(preview.Replica);
        Assert.Equal("Test line", preview.Replica.Text);
        Assert.Equal(character.Id, preview.Replica.SpeakerId);
        Assert.Empty(preview.CharactersOnScene);
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_StepWithBackground_ShouldReturnBackgroundWithCorrectTransform()
    {
        // Arrange - создать шаг с фоном
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var bgUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(bgUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bgUpload.ImageId}/confirm", null);
        
        var bgStep = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowBackgroundStepRequest(bgUpload.ImageId,
                new TransformRequest(100, 200, 1600, 900, 0.8, 15, 0)));
        Assert.NotNull(bgStep);
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{bgStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.NotNull(preview.Background);
        Assert.NotNull(preview.Background.Image);
        Assert.Equal(bgUpload.ImageId, preview.Background.Image.Id);
        
        // Проверить Transform
        Assert.NotNull(preview.Background.Transform);
        Assert.Equal(100, preview.Background.Transform.X);
        Assert.Equal(200, preview.Background.Transform.Y);
        Assert.Equal(1600, preview.Background.Transform.Width);
        Assert.Equal(900, preview.Background.Transform.Height);
        Assert.Equal(0.8, preview.Background.Transform.Scale, 2);
        Assert.Equal(15, preview.Background.Transform.Rotation);
        Assert.Equal(0, preview.Background.Transform.ZIndex);
        
        Assert.Null(preview.Replica);
        Assert.Empty(preview.CharactersOnScene);
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_StepWithOneCharacter_ShouldReturnCharacterWithCorrectTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var charImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(charImageUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{charImageUpload.ImageId}/confirm", null);
        
        var charState = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, charImageUpload.ImageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(charState);
        
        var showCharStep = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, charState.Id,
                new TransformRequest(500, 300, 400, 400, 0.9, 10, 5)));
        Assert.NotNull(showCharStep);
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{showCharStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.Single(preview.CharactersOnScene);
        
        var charOnScene = preview.CharactersOnScene.First();
        Assert.NotNull(charOnScene.State);
        Assert.NotNull(charOnScene.State.Image);
        Assert.Equal(charImageUpload.ImageId, charOnScene.State.Image.Id);
        
        // Проверить Transform
        Assert.NotNull(charOnScene.Transform);
        Assert.Equal(500, charOnScene.Transform.X);
        Assert.Equal(300, charOnScene.Transform.Y);
        Assert.Equal(400, charOnScene.Transform.Width);
        Assert.Equal(400, charOnScene.Transform.Height);
        Assert.Equal(0.9, charOnScene.Transform.Scale, 2);
        Assert.Equal(10, charOnScene.Transform.Rotation);
        Assert.Equal(5, charOnScene.Transform.ZIndex);
        
        Assert.Null(preview.Background);
        Assert.Null(preview.Replica);
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_StepWithMultipleCharacters_ShouldReturnAllCharactersInCorrectOrder()
    {
        // Arrange - создать сцену с 3 персонажами
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        // Создать 3 персонажей
        var char1 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        var char2 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Villain", "0000FF", null));
        var char3 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Sidekick", "00FF00", null));
        
        Assert.NotNull(char1);
        Assert.NotNull(char2);
        Assert.NotNull(char3);
        
        // Создать изображения и состояния для всех персонажей
        var img1 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{img1.ImageId}/confirm", null);
        var state1 = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{char1.Id}/states",
            new AddCharacterStateRequest("happy", null, img1.ImageId, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        var img2 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("villain.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{img2.ImageId}/confirm", null);
        var state2 = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{char2.Id}/states",
            new AddCharacterStateRequest("angry", null, img2.ImageId, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        var img3 = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("sidekick.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{img3.ImageId}/confirm", null);
        var state3 = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{char3.Id}/states",
            new AddCharacterStateRequest("neutral", null, img3.ImageId, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        // Добавить всех персонажей на сцену с разными z-index
        await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(char1.Id, state1.Id,
                new TransformRequest(200, 300, 512, 512, 1, 0, 1))); // z-index = 1
        
        await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(char2.Id, state2.Id,
                new TransformRequest(800, 300, 512, 512, 1, 0, 3))); // z-index = 3
        
        var lastStep = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(char3.Id, state3.Id,
                new TransformRequest(500, 300, 512, 512, 1, 0, 2))); // z-index = 2
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{lastStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.Equal(3, preview.CharactersOnScene.Count);
        
        // Проверить, что персонажи отсортированы по z-index
        var characters = preview.CharactersOnScene.ToList();
        Assert.Equal(1, characters[0].Transform.ZIndex); // Hero
        Assert.Equal(2, characters[1].Transform.ZIndex); // Sidekick
        Assert.Equal(3, characters[2].Transform.ZIndex); // Villain
        
        Assert.Null(preview.Background);
        Assert.Null(preview.Replica);
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_StepWithReplica_ShouldReturnReplicaWithTextAndSpeaker()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", "Main character"));
        Assert.NotNull(character);
        
        var replicaStep = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "This is a very important line!"));
        Assert.NotNull(replicaStep);
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{replicaStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.NotNull(preview.Replica);
        Assert.Equal("This is a very important line!", preview.Replica.Text);
        Assert.Equal(character.Id, preview.Replica.SpeakerId);
        
        Assert.Null(preview.Background);
        Assert.Empty(preview.CharactersOnScene);
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_StepWithMenu_ShouldReturnMenuWithAllChoices()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_a"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_b"));
        var label3 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_c"));
        
        Assert.NotNull(label1);
        Assert.NotNull(label2);
        Assert.NotNull(label3);
        
        var menuStep = await PostAsync<ShowMenuStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Go to path A", new ChoiceTransitionRequest { TargetLabelId = label1.Id }),
                new ChoiceRequest("Go to path B", new ChoiceTransitionRequest { TargetLabelId = label2.Id }),
                new ChoiceRequest("Go to path C", new ChoiceTransitionRequest { TargetLabelId = label3.Id })
            ]));
        Assert.NotNull(menuStep);
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{menuStep.Id}");
        
        // Assert
        Assert.NotNull(preview);
        Assert.NotNull(preview.Menu);
        Assert.Equal(3, preview.Menu.Choices.Count);
        
        Assert.Contains(preview.Menu.Choices, c => c.Text == "Go to path A");
        Assert.Contains(preview.Menu.Choices, c => c.Text == "Go to path B");
        Assert.Contains(preview.Menu.Choices, c => c.Text == "Go to path C");
        
        Assert.Null(preview.Background);
        Assert.Empty(preview.CharactersOnScene);
        Assert.Null(preview.Replica);
    }
    
    [Fact]
    public async Task Preview_CompleteScene_ShouldReturnBackgroundCharactersAndReplica()
    {
        // Arrange - создать полную сцену: фон + персонаж + реплика
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        // Фон
        var bgUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bgUpload.ImageId}/confirm", null);
        
        await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowBackgroundStepRequest(bgUpload.ImageId,
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        // Персонаж
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var charImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{charImageUpload.ImageId}/confirm", null);
        
        var charState = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, charImageUpload.ImageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, charState.Id,
                new TransformRequest(700, 200, 512, 512, 1, 0, 5)));
        
        // Реплика
        var replicaStep = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "Welcome to my world!"));
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{replicaStep.Id}");
        
        // Assert - проверить ВСЕ элементы сцены
        Assert.NotNull(preview);
        
        // Фон
        Assert.NotNull(preview.Background);
        Assert.NotNull(preview.Background.Image);
        Assert.Equal(bgUpload.ImageId, preview.Background.Image.Id);
        Assert.Equal(0, preview.Background.Transform.X);
        Assert.Equal(0, preview.Background.Transform.Y);
        
        // Персонаж
        Assert.Single(preview.CharactersOnScene);
        var charOnScene = preview.CharactersOnScene.First();
        Assert.NotNull(charOnScene.State);
        Assert.NotNull(charOnScene.State.Image);
        Assert.Equal(charImageUpload.ImageId, charOnScene.State.Image.Id);
        Assert.Equal(700, charOnScene.Transform.X);
        Assert.Equal(200, charOnScene.Transform.Y);
        Assert.Equal(5, charOnScene.Transform.ZIndex);
        
        // Реплика
        Assert.NotNull(preview.Replica);
        Assert.Equal("Welcome to my world!", preview.Replica.Text);
        Assert.Equal(character.Id, preview.Replica.SpeakerId);
        
        Assert.Null(preview.Menu);
    }
    
    [Fact]
    public async Task Preview_NonExistentStep_ShouldReturn404()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var nonExistentStepId = Guid.NewGuid();
        
        // Act
        var response = await GetRawAsync(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{nonExistentStepId}");
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Preview_AfterHideCharacter_ShouldNotShowHiddenCharacter()
    {
        // Arrange - показать персонажа, затем скрыть
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var charImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{charImageUpload.ImageId}/confirm", null);
        
        var charState = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, charImageUpload.ImageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        // Показать персонажа
        await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, charState.Id,
                new TransformRequest(500, 300, 512, 512, 1, 0, 5)));
        
        // Скрыть персонажа
        var hideStep = await PostAsync<HideCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddHideCharacterStepRequest(character.Id));
        
        // Act
        var preview = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{hideStep.Id}");
        
        // Assert - персонаж должен быть скрыт
        Assert.NotNull(preview);
        Assert.Empty(preview.CharactersOnScene);
        
        Assert.Null(preview.Background);
        Assert.Null(preview.Replica);
        Assert.Null(preview.Menu);
    }
}
