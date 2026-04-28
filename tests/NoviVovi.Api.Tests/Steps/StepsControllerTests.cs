using System.Net;
using Dapper;
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

namespace NoviVovi.Api.Tests.Steps;

[Collection("Database collection")]
public class StepsControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
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
            null,
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

    #region ShowReplica Tests

    [Fact]
    public async Task AddShowReplicaStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var request = new AddShowReplicaStepRequest(characterId, "Hello, world!");

        // Act
        var response = await PostAsync<JumpStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal(labelId, (Guid)dbStep.label_id);
        Assert.Equal("show_replica", (string)dbStep.step_type);
    }

    [Fact]
    public async Task AddShowReplicaStep_EmptyText_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var request = new AddShowReplicaStepRequest(characterId, "");

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task AddShowReplicaStep_NonExistingCharacter_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingCharacterId = Guid.NewGuid();

        var request = new AddShowReplicaStepRequest(nonExistingCharacterId, "Hello!");

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ShowCharacter Tests

    [Fact]
    public async Task AddShowCharacterStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Character);
        var stateId = await CreateTestCharacterStateAsync(novelId, characterId, imageId);

        var request = new AddShowCharacterStepRequest(
            characterId,
            stateId,
            new TransformRequest(0.5, 0.5, 512, 512, 1.0, 0.0, 1)
        );

        // Act
        var response = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal("show_character", (string)dbStep.step_type);
    }

    [Fact]
    public async Task AddShowCharacterStep_NonExistingCharacter_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingCharacterId = Guid.NewGuid();
        var nonExistingStateId = Guid.NewGuid();

        var request = new AddShowCharacterStepRequest(
            nonExistingCharacterId,
            nonExistingStateId,
            new TransformRequest(0, 0, 512, 512, 1, 0, 0)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region HideCharacter Tests

    [Fact]
    public async Task AddHideCharacterStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var request = new AddHideCharacterStepRequest(characterId);

        // Act
        var response = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal("hide_character", (string)dbStep.step_type);
    }

    [Fact]
    public async Task AddHideCharacterStep_NonExistingCharacter_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingCharacterId = Guid.NewGuid();

        var request = new AddHideCharacterStepRequest(nonExistingCharacterId);

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ShowBackground Tests

    [Fact]
    public async Task AddShowBackgroundStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var imageId = await CreateTestImageAsync(novelId, ImageTypeRequest.Background);

        var request = new AddShowBackgroundStepRequest(
            imageId,
            new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)
        );

        // Act
        var response = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal("show_background", (string)dbStep.step_type);
    }

    [Fact]
    public async Task AddShowBackgroundStep_NonExistingImage_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingImageId = Guid.NewGuid();

        var request = new AddShowBackgroundStepRequest(
            nonExistingImageId,
            new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)
        );

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region ShowMenu Tests

    [Fact]
    public async Task AddShowMenuStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var targetLabel1 = await CreateTestLabelAsync(novelId, "choice1");
        var targetLabel2 = await CreateTestLabelAsync(novelId, "choice2");

        var request = new AddShowMenuStepRequest(
            new List<ChoiceRequest>
            {
                new ChoiceRequest("Choice 1", new ChoiceTransitionRequest { TargetLabelId = targetLabel1 }),
                new ChoiceRequest("Choice 2", new ChoiceTransitionRequest { TargetLabelId = targetLabel2 })
            }
        );

        // Act
        var response = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal("show_menu", (string)dbStep.step_type);

        // Verify choices created
        var choicesCount = await QuerySingleAsync<int>(
            @"SELECT COUNT(*) FROM ""Choices"" c 
              JOIN ""Menus"" m ON c.""menu_id"" = m.""id""
              JOIN ""Steps"" s ON s.""menu_id"" = m.""id""
              WHERE s.""id"" = @StepId",
            new { StepId = response.Id });
        
        Assert.Equal(2, choicesCount);
    }

    [Fact]
    public async Task AddShowMenuStep_EmptyChoices_ReturnsUnprocessableEntity()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);

        var request = new AddShowMenuStepRequest(new List<ChoiceRequest>());

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    #endregion

    #region Jump Tests

    [Fact]
    public async Task AddJumpStep_ValidRequest_ReturnsCreatedStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var targetLabelId = await CreateTestLabelAsync(novelId, "target");

        var request = new AddJumpStepRequest(targetLabelId);

        // Act
        var response = await PostAsync<StepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbStep);
        Assert.Equal("jump", (string)dbStep.step_type);
        Assert.Equal(targetLabelId, (Guid)dbStep.next_label_id);
    }

    [Fact]
    public async Task AddJumpStep_NonExistingTargetLabel_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingLabelId = Guid.NewGuid();

        var request = new AddJumpStepRequest(nonExistingLabelId);

        // Act
        var response = await PostRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Get/GetAll/Delete Tests

    [Fact]
    public async Task GetStep_ExistingId_ReturnsStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var created = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Test"));
        Assert.NotNull(created);

        // Act
        var response = await GetAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps/{created.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(created.Id, response.Id);
    }

    [Fact]
    public async Task GetStep_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await GetRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSteps_ReturnsAllStepsForLabel()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Step 1"));
        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Step 2"));
        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Step 3"));

        // Act
        var response = await GetListAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public async Task GetAllSteps_DifferentLabels_ReturnsOnlyLabelSteps()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var label1Id = await CreateTestLabelAsync(novelId, "label1");
        var label2Id = await CreateTestLabelAsync(novelId, "label2");
        var characterId = await CreateTestCharacterAsync(novelId);

        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{label1Id}/steps",
            new AddShowReplicaStepRequest(characterId, "Label1 Step"));
        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{label2Id}/steps",
            new AddShowReplicaStepRequest(characterId, "Label2 Step"));

        // Act
        var label1Steps = await GetListAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{label1Id}/steps");
        var label2Steps = await GetListAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{label2Id}/steps");

        // Assert
        Assert.NotNull(label1Steps);
        Assert.NotNull(label2Steps);
        Assert.Single(label1Steps);
        Assert.Single(label2Steps);
    }

    [Fact]
    public async Task DeleteStep_ExistingId_DeletesStep()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        var created = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "To Delete"));
        Assert.NotNull(created);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{created.Id}");

        // Assert - verify deleted
        var getResponse = await GetRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Verify in database
        var dbStep = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""id"" = @Id",
            new { Id = created.Id });
        
        Assert.Null(dbStep);
    }

    [Fact]
    public async Task DeleteStep_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await DeleteRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StepOrder_MultipleSteps_MaintainsCorrectOrder()
    {
        // Arrange
        var novelId = await CreateTestNovelAsync();
        var labelId = await CreateTestLabelAsync(novelId);
        var characterId = await CreateTestCharacterAsync(novelId);

        // Add steps in order
        var step1 = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "First"));
        var step2 = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Second"));
        var step3 = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Third"));

        // Act - get all steps
        var steps = await GetListAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps");

        // Assert - verify order in database
        var dbSteps = await QueryAsync<dynamic>(
            @"SELECT ""id"", ""step_order"" FROM ""Steps"" 
              WHERE ""label_id"" = @LabelId 
              ORDER BY ""step_order""",
            new { LabelId = labelId });

        Assert.Equal(3, dbSteps.Count);
        Assert.Equal(step1!.Id, (Guid)dbSteps[0].id);
        Assert.Equal(step2!.Id, (Guid)dbSteps[1].id);
        Assert.Equal(step3!.Id, (Guid)dbSteps[2].id);
    }

    #endregion
}
