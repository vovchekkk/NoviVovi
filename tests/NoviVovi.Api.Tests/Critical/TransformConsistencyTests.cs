using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Critical;

/// <summary>
/// КРИТИЧЕСКИЕ ТЕСТЫ: Transform должен сохраняться во всех случаях
/// БАГ: transform_id может быть NULL в Background, ShowCharacter, StepCharacter
/// </summary>
[Collection("Database collection")]
public class TransformConsistencyTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    #region Background Transform Tests
    
    [Fact]
    public async Task CreateBackgroundStep_TransformIdShouldNotBeNull()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene"));
        Assert.NotNull(label);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        // Act - Create background step with transform
        var bgStep = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowBackgroundStepRequest(uploadInfo.ImageId, 
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        // Assert
        Assert.NotNull(bgStep);
        
        // CRITICAL: Check in database that Backgrounds.transform_id is NOT NULL
        var backgroundId = await QuerySingleAsync<Guid?>(
            @"SELECT ""background_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = bgStep.Id });
        Assert.NotNull(backgroundId);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""Backgrounds"" WHERE ""id"" = @BackgroundId",
            new { BackgroundId = backgroundId.Value });
        
        Assert.NotNull(transformId);
        Assert.NotEqual(Guid.Empty, transformId.Value);
        
        // Verify transform exists and has correct values
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x_pos"", ""y_pos"", ""width"", ""height"", ""scale"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = transformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(0, (int)transform.x_pos);
        Assert.Equal(0, (int)transform.y_pos);
        Assert.Equal(1920, (int)transform.width);
        Assert.Equal(1080, (int)transform.height);
    }
    
    [Fact]
    public async Task UpdateBackgroundStep_TransformIdShouldBeUpdated()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene"));
        Assert.NotNull(label);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var bgStep = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowBackgroundStepRequest(uploadInfo.ImageId, 
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        Assert.NotNull(bgStep);
        
        // Act - Update background step with new transform
        var updatedStep = await PatchAsync<ShowBackgroundStepResponse>(
            $"/api/novels/{novel.Id}/labels/{label.Id}/steps/{bgStep.Id}",
            new PatchShowBackgroundStepRequest(uploadInfo.ImageId,
                new TransformRequest(100, 100, 1600, 900, 0.8, 15, 5)));
        
        // Assert
        Assert.NotNull(updatedStep);
        
        // Check transform_id in database
        var backgroundId = await QuerySingleAsync<Guid?>(
            @"SELECT ""background_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = bgStep.Id });
        Assert.NotNull(backgroundId);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""Backgrounds"" WHERE ""id"" = @BackgroundId",
            new { BackgroundId = backgroundId.Value });
        
        Assert.NotNull(transformId);
        
        // Verify new transform values
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x_pos"", ""y_pos"", ""width"", ""height"", ""scale"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = transformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(100, (int)transform.x_pos);
        Assert.Equal(100, (int)transform.y_pos);
        Assert.Equal(1600, (int)transform.width);
        Assert.Equal(900, (int)transform.height);
        Assert.Equal(0.8, (double)transform.scale, 2);
        Assert.Equal(15, (int)transform.rotation);
        Assert.Equal(5, (int)transform.z_index);
    }
    
    #endregion
    
    #region ShowCharacter Transform Tests
    
    [Fact]
    public async Task CreateShowCharacterStep_TransformIdShouldNotBeNull()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene"));
        Assert.NotNull(label);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        
        // Act - Create ShowCharacter step with transform
        var showCharStep = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowCharacterStepRequest(character.Id, state.Id,
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        
        // Assert
        Assert.NotNull(showCharStep);
        
        // CRITICAL: Check in database that StepCharacter.transform_id is NOT NULL
        var stepCharacterId = await QuerySingleAsync<Guid?>(
            @"SELECT ""character_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = showCharStep.Id });
        Assert.NotNull(stepCharacterId);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""StepCharacter"" WHERE ""id"" = @StepCharacterId",
            new { StepCharacterId = stepCharacterId.Value });
        
        Assert.NotNull(transformId);
        Assert.NotEqual(Guid.Empty, transformId.Value);
        
        // Verify transform exists and has correct values
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x_pos"", ""y_pos"", ""width"", ""height"", ""scale"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = transformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(500, (int)transform.x_pos);
        Assert.Equal(200, (int)transform.y_pos);
        Assert.Equal(512, (int)transform.width);
        Assert.Equal(512, (int)transform.height);
        Assert.Equal(5, (int)transform.z_index);
    }
    
    [Fact]
    public async Task UpdateShowCharacterStep_TransformIdShouldBeUpdated()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene"));
        Assert.NotNull(label);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        
        var showCharStep = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowCharacterStepRequest(character.Id, state.Id,
                new TransformRequest(500, 200, 512, 512, 1, 0, 5)));
        Assert.NotNull(showCharStep);
        
        // Act - Update ShowCharacter step with new transform
        var updatedStep = await PatchAsync<ShowCharacterStepResponse>(
            $"/api/novels/{novel.Id}/labels/{label.Id}/steps/{showCharStep.Id}",
            new PatchShowCharacterStepRequest(character.Id, state.Id,
                new TransformRequest(800, 400, 600, 600, 0.7, 30, 10)));
        
        // Assert
        Assert.NotNull(updatedStep);
        
        // Check transform_id in database
        var stepCharacterId = await QuerySingleAsync<Guid?>(
            @"SELECT ""character_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = showCharStep.Id });
        Assert.NotNull(stepCharacterId);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""StepCharacter"" WHERE ""id"" = @StepCharacterId",
            new { StepCharacterId = stepCharacterId.Value });
        
        Assert.NotNull(transformId);
        
        // Verify new transform values
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x_pos"", ""y_pos"", ""width"", ""height"", ""scale"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = transformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(800, (int)transform.x_pos);
        Assert.Equal(400, (int)transform.y_pos);
        Assert.Equal(600, (int)transform.width);
        Assert.Equal(600, (int)transform.height);
        Assert.Equal(0.7, (double)transform.scale, 2);
        Assert.Equal(30, (int)transform.rotation);
        Assert.Equal(10, (int)transform.z_index);
    }
    
    #endregion
    
    #region Transform Deletion Tests
    
    [Fact]
    public async Task DeleteBackgroundStep_ShouldDeleteTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene"));
        Assert.NotNull(label);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("bg.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var bgStep = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowBackgroundStepRequest(uploadInfo.ImageId, 
                new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        Assert.NotNull(bgStep);
        
        var backgroundId = await QuerySingleAsync<Guid?>(
            @"SELECT ""background_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = bgStep.Id });
        Assert.NotNull(backgroundId);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""Backgrounds"" WHERE ""id"" = @BackgroundId",
            new { BackgroundId = backgroundId.Value });
        Assert.NotNull(transformId);
        
        // Act - Delete background step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps/{bgStep.Id}");
        
        // Assert - Transform should be deleted
        var transformExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Transforms"" WHERE ""id"" = @TransformId)",
            new { TransformId = transformId.Value });
        
        Assert.False(transformExists);
    }
    
    [Fact]
    public async Task DeleteCharacterState_ShouldDeleteTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("char.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""CharacterStates"" WHERE ""id"" = @StateId",
            new { StateId = state.Id });
        Assert.NotNull(transformId);
        
        // Act - Delete character state
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}/states/{state.Id}");
        
        // Assert - Transform should be deleted
        var transformExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Transforms"" WHERE ""id"" = @TransformId)",
            new { TransformId = transformId.Value });
        
        Assert.False(transformExists);
    }
    
    #endregion
}
