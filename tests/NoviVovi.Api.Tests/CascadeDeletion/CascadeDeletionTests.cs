using System.Net;
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

namespace NoviVovi.Api.Tests.CascadeDeletion;

/// <summary>
/// Tests to verify that cascade deletion works correctly for all entities.
/// This ensures data integrity and prevents orphaned records.
/// </summary>
[Collection("Database collection")]
public class CascadeDeletionTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    #region Helper Methods

    private async Task<Guid> CreateNovelAsync(string title)
    {
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest(title));
        Assert.NotNull(novel);
        return novel.Id;
    }

    private async Task<Guid> CreateLabelAsync(Guid novelId, string name)
    {
        var label = await PostAsync<LabelResponse>($"/api/novels/{novelId}/labels", new AddLabelRequest(name));
        Assert.NotNull(label);
        return label.Id;
    }

    private async Task<Guid> CreateCharacterAsync(Guid novelId, string name)
    {
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novelId}/characters",
            new AddCharacterRequest(name, "FF5733", null));
        Assert.NotNull(character);
        return character.Id;
    }

    private async Task<Guid> CreateImageAsync(Guid novelId)
    {
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novelId}/images/upload-url",
            new InitiateUploadImageRequest("test.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        await Client.PostAsync($"/api/novels/{novelId}/images/{uploadInfo.ImageId}/confirm", null);
        return uploadInfo.ImageId;
    }

    private async Task<Guid> CreateCharacterStateAsync(Guid novelId, Guid characterId, Guid imageId)
    {
        var state = await PostAsync<CharacterStateResponse>(
            $"/api/novels/{novelId}/characters/{characterId}/states",
            new AddCharacterStateRequest("happy", null, imageId, new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        return state.Id;
    }

    private async Task<Guid> CreateReplicaStepAsync(Guid novelId, Guid labelId, Guid characterId)
    {
        var step = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowReplicaStepRequest(characterId, "Test text"));
        Assert.NotNull(step);
        return step.Id;
    }

    private async Task<Guid> CreateMenuStepAsync(Guid novelId, Guid labelId, Guid targetLabelId)
    {
        var step = await PostAsync<ShowMenuStepResponse>($"/api/novels/{novelId}/labels/{labelId}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Choice 1", new ChoiceTransitionRequest { TargetLabelId = targetLabelId })
            ]));
        Assert.NotNull(step);
        return step.Id;
    }

    #endregion

    #region Novel Cascade Deletion Tests

    [Fact]
    public async Task DeleteNovel_WithLabels_DeletesAllLabels()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var label1Id = await CreateLabelAsync(novelId, "label1");
        var label2Id = await CreateLabelAsync(novelId, "label2");

        // Act
        await DeleteAsync($"/api/novels/{novelId}");

        // Assert - Verify labels are deleted
        var label1Response = await GetRawAsync($"/api/novels/{novelId}/labels/{label1Id}");
        var label2Response = await GetRawAsync($"/api/novels/{novelId}/labels/{label2Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, label1Response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, label2Response.StatusCode);

        // Verify in database
        var dbLabels = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Labels"" WHERE ""novel_id"" = @NovelId",
            new { NovelId = novelId });
        
        Assert.Empty(dbLabels);
    }

    [Fact]
    public async Task DeleteNovel_WithCharacters_DeletesAllCharacters()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var char1Id = await CreateCharacterAsync(novelId, "Character1");
        var char2Id = await CreateCharacterAsync(novelId, "Character2");

        // Act
        await DeleteAsync($"/api/novels/{novelId}");

        // Assert
        var char1Response = await GetRawAsync($"/api/novels/{novelId}/characters/{char1Id}");
        var char2Response = await GetRawAsync($"/api/novels/{novelId}/characters/{char2Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, char1Response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, char2Response.StatusCode);

        // Verify in database
        var dbCharacters = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Characters"" WHERE ""novel_id"" = @NovelId",
            new { NovelId = novelId });
        
        Assert.Empty(dbCharacters);
    }

    [Fact]
    public async Task DeleteNovel_WithImages_DeletesAllImages()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var image1Id = await CreateImageAsync(novelId);
        var image2Id = await CreateImageAsync(novelId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}");

        // Assert
        var image1Response = await GetRawAsync($"/api/novels/{novelId}/images/{image1Id}");
        var image2Response = await GetRawAsync($"/api/novels/{novelId}/images/{image2Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, image1Response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, image2Response.StatusCode);

        // Verify in database
        var dbImages = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Images"" WHERE ""novel_id"" = @NovelId",
            new { NovelId = novelId });
        
        Assert.Empty(dbImages);
    }

    [Fact]
    public async Task DeleteNovel_WithComplexStructure_DeletesEverything()
    {
        // Arrange - Create a complex novel with all entity types
        var novelId = await CreateNovelAsync("Complex Novel");
        var label1Id = await CreateLabelAsync(novelId, "label1");
        var label2Id = await CreateLabelAsync(novelId, "label2");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var imageId = await CreateImageAsync(novelId);
        var stateId = await CreateCharacterStateAsync(novelId, charId, imageId);
        var step1Id = await CreateReplicaStepAsync(novelId, label1Id, charId);
        var step2Id = await CreateMenuStepAsync(novelId, label1Id, label2Id);

        // Act
        await DeleteAsync($"/api/novels/{novelId}");

        // Assert - Verify everything is deleted
        var novelResponse = await GetRawAsync($"/api/novels/{novelId}");
        Assert.Equal(HttpStatusCode.NotFound, novelResponse.StatusCode);

        // Verify in database - no orphaned records
        var dbLabels = await QueryAsync<dynamic>(@"SELECT * FROM ""Labels"" WHERE ""novel_id"" = @NovelId", new { NovelId = novelId });
        var dbCharacters = await QueryAsync<dynamic>(@"SELECT * FROM ""Characters"" WHERE ""novel_id"" = @NovelId", new { NovelId = novelId });
        var dbImages = await QueryAsync<dynamic>(@"SELECT * FROM ""Images"" WHERE ""novel_id"" = @NovelId", new { NovelId = novelId });
        var dbSteps = await QueryAsync<dynamic>(
            @"SELECT s.* FROM ""Steps"" s 
              JOIN ""Labels"" l ON s.""label_id"" = l.""id"" 
              WHERE l.""novel_id"" = @NovelId",
            new { NovelId = novelId });
        
        Assert.Empty(dbLabels);
        Assert.Empty(dbCharacters);
        Assert.Empty(dbImages);
        Assert.Empty(dbSteps);
    }

    #endregion

    #region Label Cascade Deletion Tests

    [Fact]
    public async Task DeleteLabel_WithSteps_DeletesAllSteps()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var labelId = await CreateLabelAsync(novelId, "label1");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var step1Id = await CreateReplicaStepAsync(novelId, labelId, charId);
        var step2Id = await CreateReplicaStepAsync(novelId, labelId, charId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{labelId}");

        // Assert
        var step1Response = await GetRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{step1Id}");
        var step2Response = await GetRawAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{step2Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, step1Response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, step2Response.StatusCode);

        // Verify in database
        var dbSteps = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Steps"" WHERE ""label_id"" = @LabelId",
            new { LabelId = labelId });
        
        Assert.Empty(dbSteps);
    }

    [Fact]
    public async Task DeleteLabel_WithMenuSteps_DeletesMenusAndChoices()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var label1Id = await CreateLabelAsync(novelId, "label1");
        var label2Id = await CreateLabelAsync(novelId, "label2");
        var menuStepId = await CreateMenuStepAsync(novelId, label1Id, label2Id);

        // Get menu ID from step
        var menuId = await QuerySingleAsync<Guid>(
            @"SELECT ""menu_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = menuStepId });

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{label1Id}");

        // Assert - Verify menu and choices are deleted
        var dbMenu = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Menus"" WHERE ""id"" = @MenuId",
            new { MenuId = menuId });
        
        Assert.Null(dbMenu);

        var dbChoices = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Choices"" WHERE ""menu_id"" = @MenuId",
            new { MenuId = menuId });
        
        Assert.Empty(dbChoices);
    }

    #endregion

    #region Character Cascade Deletion Tests

    [Fact]
    public async Task DeleteCharacter_WithStates_DeletesAllStates()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var imageId = await CreateImageAsync(novelId);
        var state1Id = await CreateCharacterStateAsync(novelId, charId, imageId);
        var state2Id = await CreateCharacterStateAsync(novelId, charId, imageId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/characters/{charId}");

        // Assert
        var state1Response = await GetRawAsync($"/api/novels/{novelId}/characters/{charId}/states/{state1Id}");
        var state2Response = await GetRawAsync($"/api/novels/{novelId}/characters/{charId}/states/{state2Id}");
        
        Assert.Equal(HttpStatusCode.NotFound, state1Response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, state2Response.StatusCode);

        // Verify in database
        var dbStates = await QueryAsync<dynamic>(
            @"SELECT * FROM ""CharacterStates"" WHERE ""character_id"" = @CharacterId",
            new { CharacterId = charId });
        
        Assert.Empty(dbStates);
    }

    [Fact]
    public async Task DeleteCharacter_UsedInSteps_DeletesCascade()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var labelId = await CreateLabelAsync(novelId, "label1");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var stepId = await CreateReplicaStepAsync(novelId, labelId, charId);

        // Get replica ID before deletion
        var replicaId = await QuerySingleAsync<Guid?>(
            @"SELECT ""replica_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = stepId });
        Assert.NotNull(replicaId);

        // Act - Delete character that is used in steps (should cascade delete replicas)
        var response = await DeleteRawAsync($"/api/novels/{novelId}/characters/{charId}");

        // Assert - Should succeed with cascade deletion
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify character is deleted
        var characterResponse = await GetRawAsync($"/api/novels/{novelId}/characters/{charId}");
        Assert.Equal(HttpStatusCode.NotFound, characterResponse.StatusCode);
        
        // Verify replica is deleted (cascade)
        var dbReplica = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Replicas"" WHERE ""id"" = @ReplicaId",
            new { ReplicaId = replicaId.Value });
        Assert.Null(dbReplica);
    }

    #endregion

    #region Step Cascade Deletion Tests

    [Fact]
    public async Task DeleteStep_WithReplica_DeletesReplica()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var labelId = await CreateLabelAsync(novelId, "label1");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var stepId = await CreateReplicaStepAsync(novelId, labelId, charId);

        // Get replica ID
        var replicaId = await QuerySingleAsync<Guid?>(
            @"SELECT ""replica_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = stepId });

        Assert.NotNull(replicaId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{labelId}/steps/{stepId}");

        // Assert - Verify replica is deleted
        var dbReplica = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Replicas"" WHERE ""id"" = @ReplicaId",
            new { ReplicaId = replicaId });
        
        Assert.Null(dbReplica);
    }

    [Fact]
    public async Task DeleteStep_WithMenu_DeletesMenuAndChoices()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var label1Id = await CreateLabelAsync(novelId, "label1");
        var label2Id = await CreateLabelAsync(novelId, "label2");
        var menuStepId = await CreateMenuStepAsync(novelId, label1Id, label2Id);

        // Get menu ID
        var menuId = await QuerySingleAsync<Guid?>(
            @"SELECT ""menu_id"" FROM ""Steps"" WHERE ""id"" = @StepId",
            new { StepId = menuStepId });

        Assert.NotNull(menuId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/labels/{label1Id}/steps/{menuStepId}");

        // Assert - Verify menu and choices are deleted
        var dbMenu = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""Menus"" WHERE ""id"" = @MenuId",
            new { MenuId = menuId });
        
        Assert.Null(dbMenu);

        var dbChoices = await QueryAsync<dynamic>(
            @"SELECT * FROM ""Choices"" WHERE ""menu_id"" = @MenuId",
            new { MenuId = menuId });
        
        Assert.Empty(dbChoices);
    }

    #endregion

    #region Transform Cascade Deletion Tests

    [Fact]
    public async Task DeleteCharacterState_DeletesSuccessfully()
    {
        // Arrange
        var novelId = await CreateNovelAsync("Test Novel");
        var charId = await CreateCharacterAsync(novelId, "Character");
        var imageId = await CreateImageAsync(novelId);
        var stateId = await CreateCharacterStateAsync(novelId, charId, imageId);

        // Act
        await DeleteAsync($"/api/novels/{novelId}/characters/{charId}/states/{stateId}");

        // Assert - Verify state is deleted
        var stateResponse = await GetRawAsync($"/api/novels/{novelId}/characters/{charId}/states/{stateId}");
        Assert.Equal(HttpStatusCode.NotFound, stateResponse.StatusCode);
        
        // Verify in database
        var dbState = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""CharacterStates"" WHERE ""id"" = @StateId",
            new { StateId = stateId });
        
        Assert.Null(dbState);
    }

    #endregion
}
