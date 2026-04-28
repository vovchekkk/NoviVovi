using System.Net;
using Dapper;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Characters;

[Collection("Database collection")]
public class CharacterStatesControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Guid> CreateTestNovelAsync()
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        return novel.Id;
    }

    private async Task<Guid> CreateTestCharacterAsync(Guid novelId)
    {
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest("Alice", "FF5733", null));
        Assert.NotNull(character);
        return character.Id;
    }

    private async Task<Guid> CreateTestImageAsync(Guid novelId)
    {
        var imageRequest = new InitiateUploadImageRequest(
            "character.png",
            "Character image",
            "png",
            ImageTypeRequest.Character,
            new SizeRequest(512, 512)
        );

        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url", imageRequest);
        Assert.NotNull(uploadInfo);

        // Confirm upload (mock storage always returns true)
        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);

        return uploadInfo.ImageId;
    }

    [Fact]
    public async Task AddCharacterState_ValidRequest_ReturnsCreatedState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId);

        var request = new AddCharacterStateRequest(
            "happy",
            "Happy expression",
            imageId,
            new TransformRequest(0.5, 0.5, 512, 512, 1.0, 0.0, 1)
        );

        // Act
        var response = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("happy", response.Name);
        Assert.Equal("Happy expression", response.Description);
        Assert.NotNull(response.Image);
        Assert.Equal(imageId, response.Image.Id);
        Assert.NotNull(response.LocalTransform);
        Assert.Equal(0.5, response.LocalTransform.X);
        Assert.Equal(512, response.LocalTransform.Width);

        // Verify in database
        var dbState = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""CharacterStates"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbState);
        Assert.Equal("happy", (string)dbState.state_name);
        Assert.Equal(characterId, (Guid)dbState.character_id);
        Assert.Equal(imageId, (Guid)dbState.image_id);
    }

    [Fact]
    public async Task AddCharacterState_EmptyName_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId);

        var request = new AddCharacterStateRequest(
            "",
            null,
            imageId,
            new TransformRequest(0, 0, 512, 512, 1, 0, 0)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/characters/{characterId}/states", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacterState_NonExistingCharacter_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var nonExistingCharacterId = Guid.NewGuid();
        var imageId = await CreateTestImageAsync(novelId);

        var request = new AddCharacterStateRequest(
            "happy",
            null,
            imageId,
            new TransformRequest(0, 0, 512, 512, 1, 0, 0)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/characters/{nonExistingCharacterId}/states", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddCharacterState_NonExistingImage_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var nonExistingImageId = Guid.NewGuid();

        var request = new AddCharacterStateRequest(
            "happy",
            null,
            nonExistingImageId,
            new TransformRequest(0, 0, 512, 512, 1, 0, 0)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/characters/{characterId}/states", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task GetCharacterState_ExistingId_ReturnsState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId);

        var created = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest(
                "happy",
                null,
                imageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)
            ));
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states/{created.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("happy", response.Name);
    }

    [Fact]
    public async Task GetCharacterState_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{novelId}/characters/{characterId}/states/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCharacterStates_ReturnsAllStatesForCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var image1Id = await CreateTestImageAsync(novelId);
        var image2Id = await CreateTestImageAsync(novelId);
        var image3Id = await CreateTestImageAsync(novelId);

        var transform = new TransformRequest(0, 0, 512, 512, 1, 0, 0);

        await PostAsync<CharacterStateResponse>($"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest("happy", null, image1Id, transform));
        await PostAsync<CharacterStateResponse>($"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest("sad", null, image2Id, transform));
        await PostAsync<CharacterStateResponse>($"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest("angry", null, image3Id, transform));

        // Act
        var response = await GetListAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(3, response.Count);
        Assert.Contains(response, s => s.Name == "happy");
        Assert.Contains(response, s => s.Name == "sad");
        Assert.Contains(response, s => s.Name == "angry");
    }

    [Fact]
    public async Task PatchCharacterState_ValidRequest_UpdatesState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId);

        var created = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest(
                "happy",
                "Original",
                imageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)
            ));
        Assert.NotNull(created);

        var patchRequest = new PatchCharacterStateRequest("very_happy", "Updated description", null, null);

        // Act
        var response = await PatchAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states/{created.Id}", patchRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
        Assert.Equal("very_happy", response.Name);
        Assert.Equal("Updated description", response.Description);

        // Verify in database
        var dbState = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""CharacterStates"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.NotNull(dbState);
        Assert.Equal("very_happy", (string)dbState.state_name);
        Assert.Equal("Updated description", (string)dbState.description);
    }

    [Fact]
    public async Task PatchCharacterState_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var nonExistingId = Guid.NewGuid();
        var patchRequest = new PatchCharacterStateRequest("updated", null, null, null);

        // Act
        var response = await PatchRawAsync(
            $"/api/novels/{novelId}/characters/{characterId}/states/{nonExistingId}", patchRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCharacterState_ExistingId_DeletesState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId);

        var created = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest(
                "to_delete",
                null,
                imageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)
            ));
        Assert.NotNull(created);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/characters/{characterId}/states/{created.Id}");

        // Assert - verify deleted
        var getResponse = await GetRawAsync($"/api/novels/{novelId}/characters/{characterId}/states/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbState = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""CharacterStates"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.Null(dbState);
    }

    [Fact]
    public async Task DeleteCharacterState_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var characterId = await CreateTestCharacterAsync(novelId);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novelId}/characters/{characterId}/states/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
