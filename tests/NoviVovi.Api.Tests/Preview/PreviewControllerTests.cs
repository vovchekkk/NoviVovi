using System.Net;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;

namespace NoviVovi.Api.Tests.Preview;

[Collection("Database collection")]
public class PreviewControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Guid> CreateTestNovelAsync()
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Test Novel"));
        Assert.NotNull(novel);
        return novel.Id;
    }

    private async Task<Guid> CreateTestLabelAsync(Guid novelId, string name = "test_label")
    {
        var label = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest(name));
        Assert.NotNull(label);
        return label.Id;
    }

    private async Task<Guid> CreateTestCharacterAsync(Guid novelId, string name = "TestChar")
    {
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest(name, "FF5733", null));
        Assert.NotNull(character);
        return character.Id;
    }

    private async Task<Guid> CreateTestImageAsync(Guid novelId, ImageTypeRequest type = ImageTypeRequest.Background)
    {
        var imageRequest = new InitiateUploadImageRequest(
            "test.png",
            "png",
            type,
            new SizeRequest(512, 512)
        );

        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url", imageRequest);
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);
        return uploadInfo.ImageId;
    }

    private async Task<Guid> CreateTestCharacterStateAsync(Guid novelId, Guid characterId, Guid imageId)
    {
        var state = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest(
                "happy",
                null,
                imageId,
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)
            ));
        Assert.NotNull(state);
        return state.Id;
    }

    [Fact]
    public async Task GetPreview_SimpleReplicaStep_ReturnsSceneState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var step = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Hello, world!"));
        Assert.NotNull(step);

        // Act
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{step.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Replica);
        Assert.Equal("Hello, world!", response.Replica.Text);
    }

    [Fact]
    public async Task GetPreview_WithBackground_ReturnsSceneStateWithBackground()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Background);

        var step = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowBackgroundStepRequest(imageId, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        Assert.NotNull(step);

        // Act
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{step.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Background);
    }

    [Fact]
    public async Task GetPreview_WithCharacter_ReturnsSceneStateWithCharacter()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        var stateId = await CreateTestCharacterStateAsync(novelId, characterId, imageId);

        var step = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowCharacterStepRequest(characterId, stateId, new TransformRequest(0.5, 0.5, 512, 512, 1, 0, 1)));
        Assert.NotNull(step);

        // Act
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{step.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.CharactersOnScene);
        Assert.NotEmpty(response.CharactersOnScene);
    }

    [Fact]
    public async Task GetPreview_MultipleSteps_AccumulatesState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);
        var bgImageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Background);
        var charImageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        var stateId = await CreateTestCharacterStateAsync(novelId, characterId, charImageId);

        // Add background step
        var bgStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowBackgroundStepRequest(bgImageId, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        Assert.NotNull(bgStep);

        // Add character step
        var charStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowCharacterStepRequest(characterId, stateId, new TransformRequest(0.5, 0.5, 512, 512, 1, 0, 1)));
        Assert.NotNull(charStep);

        // Add replica step
        var replicaStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Hello!"));
        Assert.NotNull(replicaStep);

        // Act - get preview at replica step (should include background and character)
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{replicaStep.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Background);
        Assert.NotNull(response.CharactersOnScene);
        Assert.NotEmpty(response.CharactersOnScene);
        Assert.NotNull(response.Replica);
    }

    [Fact]
    public async Task GetPreview_HideCharacterStep_RemovesCharacterFromState()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        var stateId = await CreateTestCharacterStateAsync(novelId, characterId, imageId);

        // Show character
        var showStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowCharacterStepRequest(characterId, stateId, new TransformRequest(0.5, 0.5, 512, 512, 1, 0, 1)));
        Assert.NotNull(showStep);

        // Hide character
        var hideStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddHideCharacterStepRequest(characterId));
        Assert.NotNull(hideStep);

        // Act - get preview at hide step
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{hideStep.Id}");

        // Assert
        Assert.NotNull(response);
        // Character should be removed or marked as hidden
        Assert.DoesNotContain(response.CharactersOnScene, c => c.Character.Id == characterId);
    }

    [Fact]
    public async Task GetPreview_NonExistingStep_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingStepId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/preview/novels/{novelId}/labels/{labelId}/steps/{nonExistingStepId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPreview_NonExistingNovel_ReturnsNotFound()
    {
        // Arrange
        var nonExistingNovelId = Guid.NewGuid();
        var nonExistingLabelId = Guid.NewGuid();
        var nonExistingStepId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync(
            $"/preview/novels/{nonExistingNovelId}/labels/{nonExistingLabelId}/steps/{nonExistingStepId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPreview_ComplexScene_ReturnsFullState()
    {
        // Arrange - create a complex scene with background, multiple characters, and dialogue
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        
        var char1Id = await CreateTestCharacterAsync(novelId, "Alice");
        var char2Id = await CreateTestCharacterAsync(novelId, "Bob");
        
        var bgImageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Background);
        var char1ImageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        var char2ImageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        
        var char1StateId = await CreateTestCharacterStateAsync(novelId, char1Id, char1ImageId);
        var char2StateId = await CreateTestCharacterStateAsync(novelId, char2Id, char2ImageId);

        // Build scene step by step
        await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowBackgroundStepRequest(bgImageId, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        
        await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowCharacterStepRequest(char1Id, char1StateId, new TransformRequest(0.3, 0.5, 512, 512, 1, 0, 1)));
        
        await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowCharacterStepRequest(char2Id, char2StateId, new TransformRequest(0.7, 0.5, 512, 512, 1, 0, 1)));
        
        var finalStep = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(char1Id, "Hello Bob!"));
        Assert.NotNull(finalStep);

        // Act
        var response = await GetAsync<SceneStateResponse>(
            $"/preview/novels/{novelId}/labels/{labelId}/steps/{finalStep.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Background);
        Assert.NotNull(response.CharactersOnScene);
        Assert.Equal(2, response.CharactersOnScene.Count);
        Assert.NotNull(response.Replica);
    }
}
