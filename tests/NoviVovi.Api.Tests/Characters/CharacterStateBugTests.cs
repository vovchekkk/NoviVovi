using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Characters;

/// <summary>
/// Тесты для проверки критических багов с CharacterState
/// </summary>
[Collection("Database collection")]
public class CharacterStateBugTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateCharacterState_TransformIdShouldNotBeNull()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", "Test character"));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        // Act - Create character state with transform
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", "Happy expression", uploadInfo.ImageId, 
                new TransformRequest(100, 200, 512, 512, 1, 0, 5)));
        
        // Assert - State created successfully
        Assert.NotNull(state);
        Assert.Equal("happy", state.Name);
        
        // CRITICAL: Check in database that transform_id is NOT NULL
        var transformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""CharacterStates"" WHERE ""id"" = @StateId",
            new { StateId = state.Id });
        
        Assert.NotNull(transformId);
        Assert.NotEqual(Guid.Empty, transformId.Value);
        
        // Verify transform exists in Transforms table
        var transformExists = await QuerySingleAsync<bool>(
            @"SELECT EXISTS(SELECT 1 FROM ""Transforms"" WHERE ""id"" = @TransformId)",
            new { TransformId = transformId.Value });
        
        Assert.True(transformExists);
        
        // Verify transform values are correct
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x"", ""y"", ""width"", ""height"", ""opacity"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = transformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(100, (int)transform.x);
        Assert.Equal(200, (int)transform.y);
        Assert.Equal(512, (int)transform.width);
        Assert.Equal(512, (int)transform.height);
        Assert.Equal(1.0, (double)transform.opacity);
        Assert.Equal(0, (int)transform.rotation);
        Assert.Equal(5, (int)transform.z_index);
    }
    
    [Fact]
    public async Task UpdateCharacterState_TransformIdShouldBeUpdated()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        
        var originalTransformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""CharacterStates"" WHERE ""id"" = @StateId",
            new { StateId = state.Id });
        Assert.NotNull(originalTransformId);
        
        // Act - Update character state with new transform
        var updatedState = await PatchAsync<CharacterStateResponse>(
            $"/api/novels/{novel.Id}/characters/{character.Id}/states/{state.Id}",
            new PatchCharacterStateRequest("very_happy", "Very happy!", uploadInfo.ImageId,
                new TransformRequest(100, 100, 600, 600, 0.8, 45, 10)));
        
        // Assert
        Assert.NotNull(updatedState);
        
        // Check transform_id in database
        var newTransformId = await QuerySingleAsync<Guid?>(
            @"SELECT ""transform_id"" FROM ""CharacterStates"" WHERE ""id"" = @StateId",
            new { StateId = state.Id });
        
        Assert.NotNull(newTransformId);
        
        // Verify new transform values
        var transform = await QuerySingleAsync<dynamic>(
            @"SELECT ""x"", ""y"", ""width"", ""height"", ""opacity"", ""rotation"", ""z_index"" 
              FROM ""Transforms"" WHERE ""id"" = @TransformId",
            new { TransformId = newTransformId.Value });
        
        Assert.NotNull(transform);
        Assert.Equal(100, (int)transform.x);
        Assert.Equal(100, (int)transform.y);
        Assert.Equal(600, (int)transform.width);
        Assert.Equal(600, (int)transform.height);
        Assert.Equal(0.8, (double)transform.opacity, 2);
        Assert.Equal(45, (int)transform.rotation);
        Assert.Equal(10, (int)transform.z_index);
    }
    
    [Fact]
    public async Task GetCharacterState_ShouldReturnTransform()
    {
        // Arrange
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(50, 75, 400, 400, 0.9, 15, 3)));
        Assert.NotNull(state);
        
        // Act - Get character state
        var retrievedState = await GetAsync<CharacterStateResponse>(
            $"/api/novels/{novel.Id}/characters/{character.Id}/states/{state.Id}");
        
        // Assert
        Assert.NotNull(retrievedState);
        Assert.NotNull(retrievedState.LocalTransform);
        Assert.Equal(50, retrievedState.LocalTransform.X);
        Assert.Equal(75, retrievedState.LocalTransform.Y);
        Assert.Equal(400, retrievedState.LocalTransform.Width);
        Assert.Equal(400, retrievedState.LocalTransform.Height);
        Assert.Equal(0.9, retrievedState.LocalTransform.Opacity);
        Assert.Equal(15, retrievedState.LocalTransform.Rotation);
        Assert.Equal(3, retrievedState.LocalTransform.ZIndex);
    }
}
