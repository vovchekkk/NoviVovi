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
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Critical;

/// <summary>
/// КРИТИЧЕСКИЕ ТЕСТЫ: Каскадное удаление
/// Проверяют, что при удалении сущности удаляются все зависимые данные
/// </summary>
[Collection("Database collection")]
public class CascadeDeleteTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    #region CharacterState Cascade Delete
    
    [Fact]
    public async Task DeleteCharacterState_ShouldDeleteAllStepCharactersThatUseIt()
    {
        // Arrange - создать CharacterState и использовать его в ShowCharacterStep
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        
        // Создать ShowCharacterStep, который использует этот state
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, state.Id, 
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        
        // Проверить, что StepCharacter создан
        var stepCharacterExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""StepCharacter"" WHERE ""character_state_id"" = @StateId)",
            new { StateId = state.Id });
        Assert.True(stepCharacterExists);
        
        // Act - удалить CharacterState
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}/states/{state.Id}");
        
        // Assert - StepCharacter должен быть удален каскадно
        stepCharacterExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""StepCharacter"" WHERE ""character_state_id"" = @StateId)",
            new { StateId = state.Id });
        Assert.False(stepCharacterExists);
        
        // Label должен загружаться без ошибок (критично!)
        var label = await GetAsync<LabelResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}");
        Assert.NotNull(label);
    }
    
    #endregion
    
    #region Character Cascade Delete
    
    [Fact]
    public async Task DeleteCharacter_ShouldDeleteAllCharacterStates()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state1 = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        var state2 = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("sad", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        // Act - удалить Character
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}");
        
        // Assert - все CharacterStates должны быть удалены
        var state1Exists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""CharacterStates"" WHERE ""id"" = @StateId)",
            new { StateId = state1.Id });
        Assert.False(state1Exists);
        
        var state2Exists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""CharacterStates"" WHERE ""id"" = @StateId)",
            new { StateId = state2.Id });
        Assert.False(state2Exists);
    }
    
    [Fact]
    public async Task DeleteCharacter_ShouldDeleteAllReplicasWhereSpeaker()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        // Создать реплику от этого персонажа
        var step = await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "Hello!"));
        
        // Проверить, что Replica создана
        var replicaExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Replicas"" WHERE ""speaker_id"" = @CharacterId)",
            new { CharacterId = character.Id });
        Assert.True(replicaExists);
        
        // Act - удалить Character
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}");
        
        // Assert - Replica должна быть удалена или speaker_id = NULL
        replicaExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Replicas"" WHERE ""speaker_id"" = @CharacterId)",
            new { CharacterId = character.Id });
        Assert.False(replicaExists);
    }
    
    [Fact]
    public async Task DeleteCharacter_ShouldDeleteAllStepCharactersThatUseIt()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        // Создать ShowCharacterStep
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, state.Id, 
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        
        // Act - удалить Character
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}");
        
        // Assert - StepCharacter должен быть удален
        var stepCharacterExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""StepCharacter"")",
            new { });
        Assert.False(stepCharacterExists);
    }
    
    #endregion
    
    #region Label Cascade Delete
    
    [Fact]
    public async Task DeleteLabel_ShouldDeleteAllSteps()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("test_label"));
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        // Создать несколько шагов
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Step 1"));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Step 2"));
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Step 3"));
        
        // Проверить, что Steps созданы
        var stepsCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Steps"" WHERE ""label_id"" = @LabelId",
            new { LabelId = label.Id });
        Assert.Equal(3, stepsCount);
        
        // Act - удалить Label
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label.Id}");
        
        // Assert - все Steps должны быть удалены
        stepsCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Steps"" WHERE ""label_id"" = @LabelId",
            new { LabelId = label.Id });
        Assert.Equal(0, stepsCount);
    }
    
    [Fact]
    public async Task DeleteLabel_ShouldNotBreakChoicesThatReferenceIt()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label1"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label2"));
        
        // Создать Menu с выбором, который ведет на label2
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label1.Id}/steps",
            new AddShowMenuStepRequest(new List<ChoiceRequest>
            {
                new ChoiceRequest("Go to label2", new ChoiceTransitionRequest { TargetLabelId = label2.Id })
            }));
        
        // Act - удалить label2
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label2.Id}");
        
        // Assert - Choice должен быть удален или обновлен
        var choiceExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Choices"" WHERE ""next_label_id"" = @LabelId)",
            new { LabelId = label2.Id });
        Assert.False(choiceExists);
    }
    
    [Fact]
    public async Task DeleteLabel_ShouldNotBreakJumpStepsThatReferenceIt()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label1"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label2"));
        
        // Создать JumpStep, который ведет на label2
        await PostAsync<object>($"/api/novels/{novel.Id}/labels/{label1.Id}/steps",
            new AddJumpStepRequest(label2.Id));
        
        // Act - удалить label2
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label2.Id}");
        
        // Assert - JumpStep должен быть удален или обновлен
        var jumpExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Steps"" WHERE ""next_label_id"" = @LabelId)",
            new { LabelId = label2.Id });
        Assert.False(jumpExists);
    }
    
    #endregion
    
    #region Step Cascade Delete
    
    [Fact]
    public async Task DeleteShowReplicaStep_ShouldDeleteReplica()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var step = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowReplicaStepRequest(character.Id, "Hello!"));
        
        var replicaId = step.Replica.Id;
        
        // Проверить, что Replica создана
        var replicaExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Replicas"" WHERE ""id"" = @ReplicaId)",
            new { ReplicaId = replicaId });
        Assert.True(replicaExists);
        
        // Act - удалить Step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{step.Id}");
        
        // Assert - Replica должна быть удалена
        replicaExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Replicas"" WHERE ""id"" = @ReplicaId)",
            new { ReplicaId = replicaId });
        Assert.False(replicaExists);
    }
    
    [Fact]
    public async Task DeleteShowMenuStep_ShouldDeleteMenuAndChoices()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label1"));
        
        var step = await PostAsync<ShowMenuStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowMenuStepRequest(new List<ChoiceRequest>
            {
                new ChoiceRequest("Choice 1", new ChoiceTransitionRequest { TargetLabelId = label1.Id }),
                new ChoiceRequest("Choice 2", new ChoiceTransitionRequest { TargetLabelId = label1.Id })
            }));
        
        // Получить menu_id из БД через step
        var menuId = await QuerySingleAsync<Guid?>(
            @"SELECT ""menu_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = step.Id });
        Assert.NotNull(menuId);
        
        // Проверить, что Menu и Choices созданы
        var menuExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Menus"" WHERE ""id"" = @MenuId)",
            new { MenuId = menuId.Value });
        Assert.True(menuExists);
        
        var choicesCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Choices"" WHERE ""menu_id"" = @MenuId",
            new { MenuId = menuId.Value });
        Assert.Equal(2, choicesCount);
        
        // Act - удалить Step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{step.Id}");
        
        // Assert - Menu и Choices должны быть удалены
        menuExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Menus"" WHERE ""id"" = @MenuId)",
            new { MenuId = menuId.Value });
        Assert.False(menuExists);
        
        choicesCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Choices"" WHERE ""menu_id"" = @MenuId",
            new { MenuId = menuId.Value });
        Assert.Equal(0, choicesCount);
    }
    
    [Fact]
    public async Task DeleteShowBackgroundStep_ShouldDeleteBackgroundAndTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var step = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowBackgroundStepRequest(uploadInfo.ImageId, 
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        var backgroundId = step.BackgroundObject.Id;
        
        // Получить transform_id
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""Backgrounds"" WHERE ""id"" = @BackgroundId",
            new { BackgroundId = backgroundId });
        Assert.NotNull(transformId);
        
        // Act - удалить Step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{step.Id}");
        
        // Assert - Background должен быть удален
        var backgroundExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Backgrounds"" WHERE ""id"" = @BackgroundId)",
            new { BackgroundId = backgroundId });
        Assert.False(backgroundExists);
        
        // Transform должен быть удален
        var transformExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Transforms"" WHERE ""id"" = @TransformId)",
            new { TransformId = transformId.Value });
        Assert.False(transformExists);
    }
    
    [Fact]
    public async Task DeleteShowCharacterStep_ShouldDeleteStepCharacterAndTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        
        var step = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps",
            new AddShowCharacterStepRequest(character.Id, state.Id, 
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        
        var stepCharacterId = step.CharacterObject.Id;
        
        // Получить transform_id
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""StepCharacter"" WHERE ""id"" = @StepCharacterId",
            new { StepCharacterId = stepCharacterId });
        Assert.NotNull(transformId);
        
        // Act - удалить Step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{novel.StartLabelId}/steps/{step.Id}");
        
        // Assert - StepCharacter должен быть удален
        var stepCharacterExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""StepCharacter"" WHERE ""id"" = @StepCharacterId)",
            new { StepCharacterId = stepCharacterId });
        Assert.False(stepCharacterExists);
        
        // Transform должен быть удален
        var transformExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Transforms"" WHERE ""id"" = @TransformId)",
            new { TransformId = transformId.Value });
        Assert.False(transformExists);
    }
    
    #endregion
    
    #region Novel Cascade Delete
    
    [Fact]
    public async Task DeleteNovel_ShouldDeleteAllCharacters()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var char1 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        var char2 = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Villain", "0000FF", null));
        
        // Act - удалить Novel
        await DeleteAsync($"/api/novels/{novel.Id}");
        
        // Assert - все Characters должны быть удалены
        var charactersCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Characters"" WHERE ""novel_id"" = @NovelId",
            new { NovelId = novel.Id });
        Assert.Equal(0, charactersCount);
    }
    
    [Fact]
    public async Task DeleteNovel_ShouldDeleteAllLabels()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        var label1 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label1"));
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", 
            new AddLabelRequest("label2"));
        
        // Act - удалить Novel
        await DeleteAsync($"/api/novels/{novel.Id}");
        
        // Assert - все Labels должны быть удалены
        var labelsCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Labels"" WHERE ""novel_id"" = @NovelId",
            new { NovelId = novel.Id });
        Assert.Equal(0, labelsCount);
    }
    
    #endregion
}
