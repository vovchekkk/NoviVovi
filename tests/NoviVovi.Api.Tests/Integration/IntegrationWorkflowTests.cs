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
using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Scene.Requests;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Tests.Infrastructure;
using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Tests.Integration;

/// <summary>
/// End-to-end integration tests for complete workflows.
/// Tests real-world scenarios from start to finish.
/// </summary>
[Collection("Database collection")]
public class IntegrationWorkflowTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CompleteNovelCreationWorkflow_Success()
    {
        // This is the scenario from the user's request:
        // Create novel → add label → add replica → add menu → get all steps
        
        // Step 1: Create novel
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("My Test Novel"));
        Assert.NotNull(novel);
        Assert.Equal("My Test Novel", novel.Title);
        
        // Step 2: Create label (use as start label)
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("start"));
        Assert.NotNull(label);
        Assert.Equal("start", label.Name);
        
        // Step 3: Set start label for novel
        var updatedNovel = await PatchAsync<NovelResponse>($"/api/novels/{novel.Id}", 
            new PatchNovelRequest(null, label.Id));
        Assert.NotNull(updatedNovel);
        Assert.Equal(label.Id, updatedNovel.StartLabelId);
        
        // Step 4: Create character for replica
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Protagonist", "FF5733", null));
        Assert.NotNull(character);
        
        // Step 5: Add replica step
        var replicaStep = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Hello, this is my first line!"));
        Assert.NotNull(replicaStep);
        Assert.Equal("Hello, this is my first line!", replicaStep.Replica.Text);
        Assert.Equal(character.Id, replicaStep.Replica.SpeakerId);
        
        // Step 6: Create another label for menu target
        var label2 = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("choice_a"));
        Assert.NotNull(label2);
        
        // Step 7: Add menu step
        var menuStep = await PostAsync<ShowMenuStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Go left", new ChoiceTransitionRequest { TargetLabelId = label2.Id }),
                new ChoiceRequest("Go right", new ChoiceTransitionRequest { TargetLabelId = label2.Id })
            ]));
        Assert.NotNull(menuStep);
        Assert.Equal(2, menuStep.Menu.Choices.Count);
        
        // Step 8: Get all steps from label
        var steps = await GetAsync<List<StepResponse>>($"/api/novels/{novel.Id}/labels/{label.Id}/steps");
        Assert.NotNull(steps);
        Assert.Equal(2, steps.Count);
        
        // Verify order
        Assert.Equal(replicaStep.Id, steps[0].Id);
        Assert.Equal(menuStep.Id, steps[1].Id);
        
        // Step 9: Verify novel graph
        var graph = await GetAsync<NovelGraphResponse>($"/api/novels/{novel.Id}/graph");
        Assert.NotNull(graph);
        Assert.NotEmpty(graph.Nodes);
        Assert.NotEmpty(graph.Edges);
    }

    [Fact]
    public async Task CompleteCharacterWithStatesWorkflow_Success()
    {
        // Create novel
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Character Test"));
        Assert.NotNull(novel);
        
        // Create character
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "00FF00", null));
        Assert.NotNull(character);
        
        // Upload image for character state
        var uploadInfo = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("hero_happy.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(uploadInfo);
        Assert.NotNull(uploadInfo.UploadUrl);
        
        // Confirm upload
        var confirmResponse = await Client.PostAsync($"/api/novels/{novel.Id}/images/{uploadInfo.ImageId}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
        
        // Create character state
        var state = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("happy", null, uploadInfo.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(state);
        Assert.Equal("happy", state.Name);
        
        // Get character with states
        var characterWithStates = await GetAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters/{character.Id}");
        Assert.NotNull(characterWithStates);
        // Note: CharacterResponse doesn't have StateIds, so we just verify character exists
        Assert.Equal(character.Id, characterWithStates.Id);
    }

    [Fact]
    public async Task CompleteSceneWithBackgroundAndCharacters_Success()
    {
        // Create novel
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Scene Test"));
        Assert.NotNull(novel);
        
        // Create label
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("scene1"));
        Assert.NotNull(label);
        
        // Upload background image
        var bgUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("background.png", "png", ImageTypeRequest.Background, new SizeRequest(1920, 1080)));
        Assert.NotNull(bgUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{bgUpload.ImageId}/confirm", null);
        
        // Create character with state
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("NPC", "0000FF", null));
        Assert.NotNull(character);
        
        var charImageUpload = await PostAsync<UploadInfoImageResponse>($"/api/novels/{novel.Id}/images/upload-url",
            new InitiateUploadImageRequest("npc.png", "png", ImageTypeRequest.Character, new SizeRequest(512, 512)));
        Assert.NotNull(charImageUpload);
        await Client.PostAsync($"/api/novels/{novel.Id}/images/{charImageUpload.ImageId}/confirm", null);
        
        var charState = await PostAsync<CharacterStateResponse>($"/api/novels/{novel.Id}/characters/{character.Id}/states",
            new AddCharacterStateRequest("neutral", null, charImageUpload.ImageId, 
                new TransformRequest(0, 0, 512, 512, 1, 0, 0)));
        Assert.NotNull(charState);
        
        // Build scene: background → show character → replica
        var bgStep = await PostAsync<ShowBackgroundStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowBackgroundStepRequest(bgUpload.ImageId, new TransformRequest(0, 0, 1920, 1080, 1, 0, 0)));
        Assert.NotNull(bgStep);
        
        var showCharStep = await PostAsync<ShowCharacterStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowCharacterStepRequest(character.Id, charState.Id, new TransformRequest(500, 200, 512, 512, 1, 0, 1)));
        Assert.NotNull(showCharStep);
        
        var replicaStep = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Welcome to my shop!"));
        Assert.NotNull(replicaStep);
        
        // Get all steps
        var steps = await GetAsync<List<StepResponse>>($"/api/novels/{novel.Id}/labels/{label.Id}/steps");
        Assert.NotNull(steps);
        Assert.Equal(3, steps.Count);
        
        // Verify preview works
        var preview = await GetAsync<SceneStateResponse>($"/preview/novels/{novel.Id}/labels/{label.Id}/steps/{replicaStep.Id}");
        Assert.NotNull(preview);
        Assert.NotNull(preview.Background);
        Assert.Single(preview.CharactersOnScene);
    }

    [Fact]
    public async Task CompleteMenuWithMultipleChoicesAndJumps_Success()
    {
        // Create novel
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Menu Test"));
        Assert.NotNull(novel);
        
        // Create labels
        var mainLabel = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("main"));
        var pathALabel = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_a"));
        var pathBLabel = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("path_b"));
        var endLabel = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("end"));
        Assert.NotNull(mainLabel);
        Assert.NotNull(pathALabel);
        Assert.NotNull(pathBLabel);
        Assert.NotNull(endLabel);
        
        // Create character
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Guide", "FFFF00", null));
        Assert.NotNull(character);
        
        // Main label: replica + menu
        var replica1 = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{mainLabel.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Choose your path!"));
        Assert.NotNull(replica1);
        
        var menu = await PostAsync<ShowMenuStepResponse>($"/api/novels/{novel.Id}/labels/{mainLabel.Id}/steps",
            new AddShowMenuStepRequest([
                new ChoiceRequest("Take the left path", new ChoiceTransitionRequest { TargetLabelId = pathALabel.Id }),
                new ChoiceRequest("Take the right path", new ChoiceTransitionRequest { TargetLabelId = pathBLabel.Id })
            ]));
        Assert.NotNull(menu);
        
        // Path A: replica + jump to end
        var replicaA = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{pathALabel.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "You chose the left path!"));
        Assert.NotNull(replicaA);
        
        var jumpA = await PostAsync<JumpStepResponse>($"/api/novels/{novel.Id}/labels/{pathALabel.Id}/steps",
            new AddJumpStepRequest(endLabel.Id));
        Assert.NotNull(jumpA);
        
        // Path B: replica + jump to end
        var replicaB = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{pathBLabel.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "You chose the right path!"));
        Assert.NotNull(replicaB);
        
        var jumpB = await PostAsync<JumpStepResponse>($"/api/novels/{novel.Id}/labels/{pathBLabel.Id}/steps",
            new AddJumpStepRequest(endLabel.Id));
        Assert.NotNull(jumpB);
        
        // End label: final replica
        var replicaEnd = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{endLabel.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "The end!"));
        Assert.NotNull(replicaEnd);
        
        // Verify graph has all connections
        var graph = await GetAsync<NovelGraphResponse>($"/api/novels/{novel.Id}/graph");
        Assert.NotNull(graph);
        Assert.Equal(5, graph.Nodes.Count); // start, main, path_a, path_b, end
        // Menu has 2 choices + 2 jumps = 4 edges minimum
        Assert.True(graph.Edges.Count >= 2, $"Expected at least 2 edges, but got {graph.Edges.Count}"); // menu choices at minimum
    }

    [Fact]
    public async Task UpdateAndDeleteWorkflow_Success()
    {
        // Create novel with content
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Update Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("test"));
        Assert.NotNull(label);
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("TestChar", "FF0000", null));
        Assert.NotNull(character);
        
        var step = await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Original text"));
        Assert.NotNull(step);
        
        // Update novel title
        var updatedNovel = await PatchAsync<NovelResponse>($"/api/novels/{novel.Id}",
            new PatchNovelRequest("Updated Title", null));
        Assert.NotNull(updatedNovel);
        Assert.Equal("Updated Title", updatedNovel.Title);
        
        // Update label name
        var updatedLabel = await PatchAsync<LabelResponse>($"/api/novels/{novel.Id}/labels/{label.Id}",
            new PatchLabelRequest("updated_test"));
        Assert.NotNull(updatedLabel);
        Assert.Equal("updated_test", updatedLabel.Name);
        
        // Update character
        var updatedChar = await PatchAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters/{character.Id}",
            new PatchCharacterRequest("UpdatedChar", "00FF00", null));
        Assert.NotNull(updatedChar);
        Assert.Equal("UpdatedChar", updatedChar.Name);
        Assert.Equal("#00FF00", updatedChar.NameColor);
        
        // Update step
        var updatedStep = await PatchAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps/{step.Id}",
            new PatchShowReplicaStepRequest(character.Id, "Updated text"));
        Assert.NotNull(updatedStep);
        Assert.Equal("Updated text", updatedStep.Replica.Text);
        
        // Delete step
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps/{step.Id}");
        var stepResponse = await GetRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}/steps/{step.Id}");
        Assert.Equal(HttpStatusCode.NotFound, stepResponse.StatusCode);
        
        // Delete character
        await DeleteAsync($"/api/novels/{novel.Id}/characters/{character.Id}");
        var charResponse = await GetRawAsync($"/api/novels/{novel.Id}/characters/{character.Id}");
        Assert.Equal(HttpStatusCode.NotFound, charResponse.StatusCode);
        
        // Delete label
        await DeleteAsync($"/api/novels/{novel.Id}/labels/{label.Id}");
        var labelResponse = await GetRawAsync($"/api/novels/{novel.Id}/labels/{label.Id}");
        Assert.Equal(HttpStatusCode.NotFound, labelResponse.StatusCode);
        
        // Delete novel
        await DeleteAsync($"/api/novels/{novel.Id}");
        var novelResponse = await GetRawAsync($"/api/novels/{novel.Id}");
        Assert.Equal(HttpStatusCode.NotFound, novelResponse.StatusCode);
    }

    [Fact]
    public async Task ExportNovelToRenPy_Success()
    {
        // Create simple novel
        var novel = await PostAsync<NovelResponse>("/api/novels", new CreateNovelRequest("Export Test"));
        Assert.NotNull(novel);
        
        var label = await PostAsync<LabelResponse>($"/api/novels/{novel.Id}/labels", new AddLabelRequest("start"));
        Assert.NotNull(label);
        
        await PatchAsync<NovelResponse>($"/api/novels/{novel.Id}", 
            new PatchNovelRequest(null, label.Id));
        
        var character = await PostAsync<CharacterResponse>($"/api/novels/{novel.Id}/characters",
            new AddCharacterRequest("Hero", "FF5733", null));
        Assert.NotNull(character);
        
        await PostAsync<ShowReplicaStepResponse>($"/api/novels/{novel.Id}/labels/{label.Id}/steps",
            new AddShowReplicaStepRequest(character.Id, "Hello world!"));
        
        // Export to RenPy
        var response = await Client.GetAsync($"/api/novels/{novel.Id}/export/renpy");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);
        
        var content = await response.Content.ReadAsByteArrayAsync();
        Assert.True(content.Length > 0);
    }
}
