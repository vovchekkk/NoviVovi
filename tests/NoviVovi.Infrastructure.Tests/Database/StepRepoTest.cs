using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Repositories;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;

namespace NoviVovi.Infrastructure.Tests.Database;

[Collection("Sequential")]
public class StepRepoTest : IAsyncLifetime
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private readonly Dictionary<string, HashSet<Guid>> idsToDelete = new();
    private readonly List<string> deleteOrder =
    [
        "StepCharacter",
        "Steps",
        "CharacterStates",
        "Characters",
        "Choices",
        "Menus",
        "Replicas",
        "Backgrounds",
        "Labels",
        "Images",
        "Transforms",
        "Novels"
    ];
    
    private Guid novelId;
    private Guid labelId;
    
    private readonly ILabelDbORepository labelRepo;
    private readonly IMenuDbORepository menuRepo;
    private readonly IStepDbORepository stepRepo;
    private readonly ICharacterDbORepository characterRepo;
    private readonly IImageDbORepository imageRepo;
    
    public StepRepoTest()
    {
        provider = TestHelper.CreateProvider();
        labelRepo = provider.GetRequiredService<ILabelDbORepository>();
        menuRepo = provider.GetRequiredService<IMenuDbORepository>();
        stepRepo = provider.GetRequiredService<IStepDbORepository>();
        characterRepo = provider.GetRequiredService<ICharacterDbORepository>();
        imageRepo = provider.GetRequiredService<IImageDbORepository>();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
        
        foreach (var table in deleteOrder)
        {
            idsToDelete[table] = [];
        }
    }

    public async Task InitializeAsync()
    {
        novelId = Guid.NewGuid();
        TrackId("Novels", novelId);
        
        const string insertNovelSql = @"
            INSERT INTO ""Novels"" (id, title, is_public, created_at, edited_at)
            VALUES (@Id, @Title, @IsPublic, NOW(), NOW())";
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(insertNovelSql, new
        {
            Id = novelId,
            Title = "Test Novel for Steps",
            IsPublic = true
        });
        
        labelId = Guid.NewGuid();
        TrackId("Labels", labelId);
        
        const string insertLabelSql = @"
            INSERT INTO ""Labels"" (id, novel_id, label_name)
            VALUES (@Id, @NovelId, @LabelName)";
        
        await conn.ExecuteAsync(insertLabelSql, new
        {
            Id = labelId,
            NovelId = novelId,
            LabelName = "test_label_for_steps"
        });
    }
    
    private void TrackId(string tableName, Guid id)
    {
        if (idsToDelete.ContainsKey(tableName))
            idsToDelete[tableName].Add(id);
    }
    
    private async Task DeleteFromTableAsync(string tableName, IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        if (!idList.Any()) return;
        var deleteSql = $"DELETE FROM \"{tableName}\" WHERE id = ANY(@Ids)";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(deleteSql, new { Ids = idList });
    }
    
    // ==================== Вспомогательные конструкторы ====================
    
    private LabelDbO CreateLabel(string labelName = "test_label")
    {
        var label = new LabelDbO
        {
            Id = Guid.NewGuid(),
            NovelId = novelId,
            LabelName = labelName
        };
        TrackId("Labels", label.Id);
        return label;
    }

    private MenuDbO CreateMenu()
    {
        var menu = new MenuDbO
        {
            Id = Guid.NewGuid()
        };
        TrackId("Menus", menu.Id);
        return menu;
    }

    private ChoiceDbO CreateChoice(Guid menuId, LabelDbO nextLabel)
    {
        var choice = new ChoiceDbO
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            NextLabel = nextLabel,
            NextLabelId = nextLabel.Id,
            Text = "test_choice_text"
        };
        TrackId("Choices", choice.Id);
        return choice;
    }
    
    private ReplicaDbO CreateReplica(string text = "test replica text", Guid? speakerId = null)
    {
        var replica = new ReplicaDbO
        {
            Id = Guid.NewGuid(),
            Text = text,
            SpeakerId = speakerId
        };
        TrackId("Replicas", replica.Id);
        return replica;
    }
    
    private TransformDbO CreateTransform(int width = 100, int height = 100)
    {
        var transform = new TransformDbO
        {
            Id = Guid.NewGuid(),
            Width = width,
            Height = height,
            Scale = 1,
            XPos = 0,
            YPos = 0,
            Rotation = 0,
            ZIndex = 0
        };
        TrackId("Transforms", transform.Id);
        return transform;
    }
    
    private ImageDbO CreateImage(string name = "test_image", string imgType = "background")
    {
        var image = new ImageDbO
        {
            Id = Guid.NewGuid(),
            NovelId = novelId,
            Name = name,
            Url = "http://test.com/image.png",
            Format = "png",
            ImgType = imgType,
            Height = 200,
            Width = 200,
            Size = 1024
        };
        TrackId("Images", image.Id);
        return image;
    }
    
    private BackgroundDbO CreateBackground(ImageDbO image, TransformDbO? transform = null)
    {
        var bg = new BackgroundDbO
        {
            Id = Guid.NewGuid(),
            Img = image.Id,
            Image = image,
            TransformId = transform?.Id,
            Transform = transform
        };
        TrackId("Backgrounds", bg.Id);
        return bg;
    }
    
    private CharacterDbO CreateCharacter(string name = "Test Character")
    {
        var character = new CharacterDbO
        {
            Id = Guid.NewGuid(),
            NovelId = novelId,
            Name = name,
            NameColor = "000000",
            Description = "test description"
        };
        TrackId("Characters", character.Id);
        return character;
    }
    
    private CharacterStateDbO CreateCharacterState(CharacterDbO character, ImageDbO image, TransformDbO? transform = null, string stateName = "idle")
    {
        var state = new CharacterStateDbO
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            ImageId = image.Id,
            Image = image,
            StateName = stateName,
            Description = "state description",
            TransformId = transform?.Id,
            Transform = transform
        };
        TrackId("CharacterStates", state.Id);
        return state;
    }
    
    private StepCharacterDbO CreateStepCharacter(CharacterStateDbO state, TransformDbO? transform = null)
    {
        var stepChar = new StepCharacterDbO
        {
            Id = Guid.NewGuid(),
            CharacterStateId = state.Id,
            CharacterState = state,
            TransformId = transform?.Id,
            Transform = transform
        };
        TrackId("StepCharacter", stepChar.Id);
        return stepChar;
    }
    
    private StepDbO CreateStep(Guid labelId, int order, string stepType, 
        ReplicaDbO? replica = null, MenuDbO? menu = null, BackgroundDbO? background = null,
        StepCharacterDbO? character = null, LabelDbO? nextLabel = null)
    {
        var step = new StepDbO
        {
            Id = Guid.NewGuid(),
            LabelId = labelId,
            StepOrder = order,
            StepType = stepType,
            Replica = replica,
            Menu = menu,
            Background = background,
            Character = character,
            NextLabel = nextLabel,
            
            NextLabelId = nextLabel?.Id,
            CharacterId = character?.Id,
            ReplicaId = replica?.Id,
            BackgroundId = background?.Id,
            MenuId = menu?.Id
        };
        TrackId("Steps", step.Id);
        return step;
    }
    
    // ==================== Тесты реплик ====================
    
    [Fact]
    public async Task TestCreateReplica()
    {
        var replica = CreateReplica("hello world");
        
        var result = await stepRepo.CreateReplicaAsync(replica);
        
        Assert.Equal(replica.Id, result);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbText = await conn.ExecuteScalarAsync<string>("SELECT text FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal("hello world", dbText);
    }
    
    [Fact]
    public async Task TestGetReplicaById()
    {
        var replica = CreateReplica("test get");
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(
            "INSERT INTO \"Replicas\" (id, speaker_id, text) VALUES (@Id, @SpeakerId, @Text)",
            new { replica.Id, SpeakerId = replica.SpeakerId, replica.Text });
        
        var result = await stepRepo.GetReplicaByIdAsync(replica.Id);
        
        Assert.NotNull(result);
        Assert.Equal(replica.Id, result.Id);
        Assert.Equal(replica.Text, result.Text);
    }
    
    [Fact]
    public async Task TestDeleteReplica()
    {
        var replica = CreateReplica("to delete");
        await stepRepo.CreateReplicaAsync(replica);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var before = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal(1, before);
        
        await stepRepo.DeleteReplicaAsync(replica.Id);
        idsToDelete["Replicas"].Remove(replica.Id);
        
        var after = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestUpdateReplica()
    {
        var character = CreateCharacter("Speaker");
        await characterRepo.AddOrUpdateFullAsync(character);
        
        var replica = CreateReplica("old text");
        await stepRepo.CreateReplicaAsync(replica);
        
        replica.Text = "new text";
        replica.SpeakerId = character.Id;
        await stepRepo.UpdateReplicaAsync(replica);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var (text, speakerId) = await conn.QueryFirstOrDefaultAsync<(string, Guid?)>(
            "SELECT text, speaker_id FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal("new text", text);
        Assert.Equal(character.Id, speakerId);
    }
    
    [Fact]
    public async Task TestShowReplicaStep()
    {
        var label = CreateLabel("step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var replica = CreateReplica("step replica text");
        var step = CreateStep(label.Id, 1, "replica", replica: replica);
        
        // Создаём
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStep = await conn.QueryFirstOrDefaultAsync<(string stepType, Guid? replicaId)>(
            "SELECT step_type, replica_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("replica", dbStep.stepType);
        Assert.Equal(replica.Id, dbStep.replicaId);
        
        // Обновляем реплику
        replica.Text = "updated text";
        await stepRepo.UpdateReplicaAsync(replica);
        var dbText = await conn.ExecuteScalarAsync<string>("SELECT text FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal("updated text", dbText);
        
        // Удаляем степ — реплика не удаляется (нет каскада)
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
        var replicaStillExists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Replicas\" WHERE id = @Id", new { Id = replica.Id });
        Assert.Equal(1, replicaStillExists);
    }
    
    // ==================== Тесты меню-степов ====================
    
    [Fact]
    public async Task TestShowMenuStep()
    {
        var label = CreateLabel("menu_step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var menu = CreateMenu();
        var choiceLabel = CreateLabel("choice_label");
        await labelRepo.AddOrUpdateFullAsync(choiceLabel);
        var choice = CreateChoice(menu.Id, choiceLabel);
        menu.Choices = [choice];
        
        var step = CreateStep(label.Id, 1, "menu", menu: menu);
        
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStep = await conn.QueryFirstOrDefaultAsync<(string stepType, Guid? menuId)>(
            "SELECT step_type, menu_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("menu", dbStep.stepType);
        Assert.Equal(menu.Id, dbStep.menuId);
        
        // Обновляем меню
        choice.Text = "aboba";
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        var choiceText =
            await conn.ExecuteScalarAsync<string>("SELECT text FROM \"Choices\" WHERE id = @Id", new { Id = choice.Id });
        Assert.Equal("aboba", choiceText);
        
        // Удаляем степ — меню не удаляется
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
        var menuExists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Menus\" WHERE id = @Id", new { Id = menu.Id });
        Assert.Equal(1, menuExists);
    }
    
    // ==================== Тесты фонов ====================
    
    [Fact]
    public async Task TestCreateBackground()
    {
        var image = CreateImage();
        var transform = CreateTransform();
        var bg = CreateBackground(image, transform);
        
        var result = await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        Assert.Equal(bg.Id, result);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbBg = await conn.QueryFirstOrDefaultAsync<(Guid img, Guid? transformId)>(
            "SELECT img, transform_id FROM \"Backgrounds\" WHERE id = @Id", new { Id = bg.Id });
        Assert.Equal(image.Id, dbBg.img);
        Assert.Equal(transform.Id, dbBg.transformId);
    }
    
    [Fact]
    public async Task TestGetBackground()
    {
        var image = CreateImage();
        var transform = CreateTransform();
        var bg = CreateBackground(image, transform);
        
        await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        var result = await imageRepo.GetFullBackgroundByIdAsync(bg.Id);
        
        Assert.NotNull(result);
        Assert.Equal(bg.Id, result.Id);
        Assert.NotNull(result.Image);
        Assert.Equal(image.Id, result.Image.Id);
        Assert.NotNull(result.Transform);
        Assert.Equal(transform.Id, result.Transform.Id);
    }
    
    [Fact]
    public async Task TestUpdateBackground()
    {
        var image1 = CreateImage("bg1");
        var image2 = CreateImage("bg2");
        var transform = CreateTransform();
        var bg = CreateBackground(image1, transform);
        await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        await imageRepo.AddOrUpdateImageAsync(image2);
        bg.Img = image2.Id;
        bg.Image = image2;
        await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        var updated = await imageRepo.GetFullBackgroundByIdAsync(bg.Id);
        Assert.Equal(image2.Id, updated.Img);
    }
    
    [Fact]
    public async Task TestDeleteBackground()
    {
        var image = CreateImage();
        var bg = CreateBackground(image);
        await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync("DELETE FROM \"Backgrounds\" WHERE id = @Id", new { Id = bg.Id });
        idsToDelete["Backgrounds"].Remove(bg.Id);
        
        var exists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Backgrounds\" WHERE id = @Id", new { Id = bg.Id });
        Assert.Equal(0, exists);
    }
    
    [Fact]
    public async Task TestShowBackgroundStep()
    {
        var label = CreateLabel("bg_step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var image = CreateImage();
        var transform = CreateTransform();
        var bg = CreateBackground(image, transform);
        await imageRepo.AddOrUpdateBackgroundAsync(bg);
        
        var step = CreateStep(label.Id, 1, "background", background: bg);
        
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStep = await conn.QueryFirstOrDefaultAsync<(string stepType, Guid? backgroundId)>(
            "SELECT step_type, background_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("background", dbStep.stepType);
        Assert.Equal(bg.Id, dbStep.backgroundId);
        
        // Обновляем фон
        var image2 = CreateImage("bg2");
        var bg2 = CreateBackground(image2);
        await imageRepo.AddOrUpdateBackgroundAsync(bg2);
        step.Background = bg2;
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        var updatedBgId = await conn.ExecuteScalarAsync<Guid?>("SELECT background_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal(bg2.Id, updatedBgId);
        
        // Удаляем степ — фон не удаляется
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
        var bgExists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"Backgrounds\" WHERE id = @Id", new { Id = bg.Id });
        Assert.Equal(1, bgExists);
    }
    
    // ==================== Тесты персонажей (StepCharacter) ====================
    
    [Fact]
    public async Task TestCreateStepCharacter()
    {
        var character = CreateCharacter();
        await characterRepo.AddOrUpdateFullAsync(character);
        var image = CreateImage();
        var state = CreateCharacterState(character, image);
        await characterRepo.AddOrUpdateStateAsync(state);
        
        var stepChar = CreateStepCharacter(state);
        
        var result = await characterRepo.AddOrUpdateStepCharacterAsync(stepChar);
        
        Assert.Equal(stepChar.Id, result);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStateId = await conn.ExecuteScalarAsync<Guid>("SELECT character_state_id FROM \"StepCharacter\" WHERE id = @Id", new { Id = stepChar.Id });
        Assert.Equal(state.Id, dbStateId);
    }
    
    [Fact]
    public async Task TestGetStepCharacter()
    {
        var character = CreateCharacter();
        await characterRepo.AddOrUpdateFullAsync(character);
        var image = CreateImage();
        var state = CreateCharacterState(character, image);
        await characterRepo.AddOrUpdateStateAsync(state);
        
        var stepChar = CreateStepCharacter(state);
        await characterRepo.AddOrUpdateStepCharacterAsync(stepChar);
        
        var result = await characterRepo.GetFullStepCharacterByIdAsync(stepChar.Id);
        
        Assert.NotNull(result);
        Assert.Equal(stepChar.Id, result.Id);
        Assert.NotNull(result.CharacterState);
        Assert.Equal(state.Id, result.CharacterState.Id);
    }
    
    [Fact]
    public async Task TestUpdateStepCharacter()
    {
        var character1 = CreateCharacter("Char1");
        await characterRepo.AddOrUpdateFullAsync(character1);
        var image1 = CreateImage();
        var state1 = CreateCharacterState(character1, image1, stateName: "state1");
        await characterRepo.AddOrUpdateStateAsync(state1);
        
        var character2 = CreateCharacter("Char2");
        await characterRepo.AddOrUpdateFullAsync(character2);
        var image2 = CreateImage();
        var state2 = CreateCharacterState(character2, image2, stateName: "state2");
        await characterRepo.AddOrUpdateStateAsync(state2);
        
        var stepChar = CreateStepCharacter(state1);
        await characterRepo.AddOrUpdateStepCharacterAsync(stepChar);
        
        stepChar.CharacterStateId = state2.Id;
        stepChar.CharacterState = state2;
        await characterRepo.AddOrUpdateStepCharacterAsync(stepChar);
        
        var updated = await characterRepo.GetFullStepCharacterByIdAsync(stepChar.Id);
        Assert.Equal(state2.Id, updated.CharacterState.Id);
    }
    
    [Fact]
    public async Task TestDeleteStepCharacter()
    {
        var character = CreateCharacter();
        await characterRepo.AddOrUpdateFullAsync(character);
        var image = CreateImage();
        var state = CreateCharacterState(character, image);
        await characterRepo.AddOrUpdateStateAsync(state);
        
        var stepChar = CreateStepCharacter(state);
        await characterRepo.AddOrUpdateStepCharacterAsync(stepChar);
        
        await using var conn = new NpgsqlConnection(connectionString);
        var before = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"StepCharacter\" WHERE id = @Id", new { Id = stepChar.Id });
        Assert.Equal(1, before);
        
        await characterRepo.DeleteStepCharacterAsync(stepChar.Id);
        idsToDelete["StepCharacter"].Remove(stepChar.Id);
        
        var after = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"StepCharacter\" WHERE id = @Id", new { Id = stepChar.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestShowCharacterStep()
    {
        var label = CreateLabel("char_step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var character = CreateCharacter();
        await characterRepo.AddOrUpdateFullAsync(character);
        var image = CreateImage();
        var state = CreateCharacterState(character, image);
        await characterRepo.AddOrUpdateStateAsync(state);
        
        var stepChar = CreateStepCharacter(state);
        var step = CreateStep(label.Id, 1, "show_character", character: stepChar);
        
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStep = await conn.QueryFirstOrDefaultAsync<(string stepType, Guid? characterId)>(
            "SELECT step_type, character_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("show_character", dbStep.stepType);
        Assert.Equal(stepChar.Id, dbStep.characterId);
        
        // Обновляем персонажа в шаге
        var character2 = CreateCharacter("new char");
        await characterRepo.AddOrUpdateFullAsync(character2);
        var image2 = CreateImage();
        var state2 = CreateCharacterState(character2, image2);
        await characterRepo.AddOrUpdateStateAsync(state2);
        var stepChar2 = CreateStepCharacter(state2);
        step.Character = stepChar2;
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        var updatedCharId = await conn.ExecuteScalarAsync<Guid?>("SELECT character_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal(stepChar2.Id, updatedCharId);
        
        // Удаляем степ — StepCharacter не удаляется
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
        var stepCharExists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"StepCharacter\" WHERE id = @Id", new { Id = stepChar.Id });
        Assert.Equal(1, stepCharExists);
    }
    
    [Fact]
    public async Task TestHideCharacterStep()
    {
        var label = CreateLabel("hide_char_step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        
        var step = CreateStep(label.Id, 1, "hide_character");
        
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var stepType = await conn.ExecuteScalarAsync<string>("SELECT step_type FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("hide_character", stepType);
        
        var details = await conn.QueryFirstOrDefaultAsync<(Guid? replicaId, Guid? menuId, Guid? bgId, Guid? characterId)>(
            "SELECT replica_id, menu_id, background_id, character_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Null(details.replicaId);
        Assert.Null(details.menuId);
        Assert.Null(details.bgId);
        Assert.Null(details.characterId);
        
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
    }
    
    // ==================== Тесты перехода ====================
    
    [Fact]
    public async Task TestJumpStep()
    {
        var label = CreateLabel("jump_step_label");
        await labelRepo.AddOrUpdateFullAsync(label);
        var nextLabel = CreateLabel("next_label");
        await labelRepo.AddOrUpdateFullAsync(nextLabel);
        
        var step = CreateStep(label.Id, 1, "jump", nextLabel: nextLabel);
        
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        await using var conn = new NpgsqlConnection(connectionString);
        var dbStep = await conn.QueryFirstOrDefaultAsync<(string stepType, Guid? nextLabelId)>(
            "SELECT step_type, next_label_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal("jump", dbStep.stepType);
        Assert.Equal(nextLabel.Id, dbStep.nextLabelId);
        
        // Обновляем nextLabel
        var anotherLabel = CreateLabel("another_label");
        await labelRepo.AddOrUpdateFullAsync(anotherLabel);
        step.NextLabel = anotherLabel;
        await stepRepo.AddOrUpdateFullAsync(step, new LoadContext());
        
        var updatedNext = await conn.ExecuteScalarAsync<Guid?>("SELECT next_label_id FROM \"Steps\" WHERE id = @Id", new { Id = step.Id });
        Assert.Equal(anotherLabel.Id, updatedNext);
        
        await stepRepo.DeleteAsync(step.Id);
        idsToDelete["Steps"].Remove(step.Id);
    }

    public async Task DisposeAsync()
    {
        foreach (var tableName in deleteOrder)
        {
            if (idsToDelete.TryGetValue(tableName, out var ids) && ids.Any())
            {
                await DeleteFromTableAsync(tableName, ids);
            }
        }
    }
}