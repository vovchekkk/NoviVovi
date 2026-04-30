using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;

namespace NoviVovi.Infrastructure.Tests.Database;

[Collection("Sequential")]
public class CharacterRepoTest : IAsyncLifetime
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private readonly ICharacterDbORepository repo;
    
    private Guid novelId;
    private readonly HashSet<Guid> statesIds = [];
    private readonly HashSet<Guid> transformsIds = [];
    private readonly HashSet<Guid> imagesIds = [];
    private readonly HashSet<Guid> characterIds = [];
    
    public CharacterRepoTest()
    {
        provider = TestHelper.CreateProvider();
        repo = provider.GetService<ICharacterDbORepository>();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
    }
    
    public async Task InitializeAsync()
    {
        novelId = Guid.NewGuid();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        const string insertNovelSql = @"INSERT INTO ""Novels"" (id, is_public, title, created_at, edited_at) 
                                        VALUES (@Id, @IsPublic, @Title, @CreatedAt, @EditedAt)";
        
        var novel = new NovelDbO
        {
            Id = novelId,
            IsPublic = true,
            Title = "abobiki",
            CreatedAt = DateTime.UtcNow,
            EditedAt = DateTime.UtcNow
        };
        await conn.ExecuteAsync(insertNovelSql, novel);
    }

    private CharacterDbO CreateCharacter()
    {
        var guid = Guid.NewGuid();
        characterIds.Add(guid);
        var character = new CharacterDbO
        {
            Id = guid,
            Description = "abobik",
            Name = "Abobus Abobusovich",
            NovelId = novelId,
            NameColor = "000000"
        };
        return character;
    }
    
    private async Task DeleteFromTableAsync(string tableName, IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        if (!idList.Any()) return;
        
        var deleteSql = $"DELETE FROM \"{tableName}\" WHERE id = ANY(@Ids)";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(deleteSql, new { Ids = idList.ToArray() });
    }
    
    private ImageDbO CreateImage(Guid novelId)
    {
        var imageId = Guid.NewGuid();

        var testImage = new ImageDbO
        {
            Id = imageId,
            NovelId = novelId,
            Name = "test",
            Url = "test/img",
            Format = "png",
            ImgType = "background",
            Height = 100,
            Width = 100,
            Size = 1
        };
        imagesIds.Add(imageId);
        return testImage;
    }
    
    private TransformDbO CreateTransform()
    {
        var id = Guid.NewGuid();
        var transform = new TransformDbO
        {
            Id = id,
            Height = 100,
            Width = 100,
            Scale = 1,
            XPos = 0,
            YPos = 0
        };
        transformsIds.Add(id);
        return transform;
    }
    
    private CharacterStateDbO CreateCharacterState(CharacterDbO character, ImageDbO image, TransformDbO transform)
    {
        var state = new CharacterStateDbO
        {
            Id = Guid.NewGuid(),
            StateName = "abobik",
            CharacterId = character.Id,
            ImageId = image.Id,
            Image = image,
            TransformId = transform.Id,
            Transform = transform
        };
        statesIds.Add(state.Id);
        return state;
    }

    private List<CharacterStateDbO> GetCharacterStates( //хуйня, которая позволяет делать много однотипных стейтов
        IEnumerable<string> names,
        CharacterDbO character,
        ImageDbO image,
        TransformDbO transform)
    {
        var res = new List<CharacterStateDbO>();
        foreach (var name in names)
        {
            var state = new CharacterStateDbO
            {
                Id = Guid.NewGuid(),
                StateName = name,
                CharacterId = character.Id,
                ImageId = image.Id,
                Image = image,
                TransformId = transform.Id,
                Transform = transform
            };
            statesIds.Add(state.Id);
            res.Add(state);
        }

        return res;
    }
    
    [Fact]
    public async Task TestGetCharacterByIdAsync()
    {
        var sql = @"INSERT INTO ""Characters"" (id, description, name, novel_id, name_color) 
            VALUES (@Id, @Description, @Name, @NovelId, @NameColor)";
        var character = CreateCharacter();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(sql, character);

        var res = await repo.GetFullCharacterByIdAsync(character.Id);
        Assert.NotNull(res);
        Assert.Equal(character.Id, res.Id);
        Assert.Equal(character.Description, res.Description);
        Assert.Equal(character.Name, res.Name);
        Assert.Equal(character.NovelId, res.NovelId);
        Assert.Equal(character.NameColor, res.NameColor);
    }

    [Fact]
    public async Task TestCreateCharacterAsync() //будет работать корректно только если предыдущий тест выполнился правильно 
    {
        var character = CreateCharacter();
        const string countScript = @"SELECT COUNT(*) FROM ""Characters"" WHERE id = @Id";
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = character.Id });
        Assert.Equal(0, before);
        await repo.AddAsync(character);
        
        var res = await repo.GetFullCharacterByIdAsync(character.Id);
        
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = character.Id });
        Assert.True(after > 0);
        
        Assert.NotNull(res);
        Assert.Equal(character.Id, res.Id);
        Assert.Equal(character.Description, res.Description);
        Assert.Equal(character.Name, res.Name);
        Assert.Equal(character.NovelId, res.NovelId);
        Assert.Equal(character.NameColor, res.NameColor);
    }
    
    [Fact]
    public async Task TestDeleteCharacterAsync()
    {
        var character = CreateCharacter();
        const string countScript = @"SELECT COUNT(*) FROM ""Characters"" WHERE id = @Id";
        
        await repo.AddAsync(character);
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = character.Id });
        Assert.True(before > 0);
        await repo.DeleteAsync(character.Id);
        
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = character.Id });
        Assert.Equal(0, after);
    }
    
    [Fact]
    public async Task TestUpdateCharacterAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        // меняем данные
        character.Name = "Updated Name";
        character.Description = "Updated Description";
        character.NameColor = "FFFFFF";

        await repo.AddOrUpdateFullAsync(character);

        var result = await repo.GetFullCharacterByIdAsync(character.Id);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal("FFFFFF", result.NameColor);
    }
    
    [Fact]
    public async Task TestGetByIdStateAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);
        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
                              VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var state = CreateCharacterState(character, image, transform);

        await conn.ExecuteAsync(@"INSERT INTO ""CharacterStates"" 
        (id, character_id, image_id, state_name, transform_id)
        VALUES (@Id, @CharacterId, @ImageId, @StateName, @TransformId)", state);

        var result = await repo.GetFullCharacterStateByIdAsync(state.Id);

        Assert.NotNull(result);
        Assert.Equal(state.Id, result.Id);
        Assert.NotNull(result.Image);
        Assert.NotNull(result.Transform);
    }
    
    [Fact]
    public async Task TestCreateStateAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);
        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
                              VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var state = CreateCharacterState(character, image, transform);

        await repo.AddOrUpdateStateAsync(state);

        var result = await repo.GetFullCharacterStateByIdAsync(state.Id);

        Assert.NotNull(result);
        Assert.Equal(state.Id, result.Id);
    }
    
    [Fact]
    public async Task TestDeleteStateAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);
        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
                              VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var state = CreateCharacterState(character, image, transform);

        await repo.AddOrUpdateStateAsync(state);

        await repo.DeleteStateAsync(state.Id);

        var result = await repo.GetFullCharacterStateByIdAsync(state.Id);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task TestUpdateStateAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" 
        VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);

        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" 
        (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
        VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var state = CreateCharacterState(character, image, transform);

        character.States = new List<CharacterStateDbO> { state };
        await repo.AddOrUpdateFullAsync(character);
        
        state.StateName = "updated_state";
        state.Description = "updated_desc";

        await repo.AddOrUpdateFullAsync(character);

        var result = await repo.GetFullCharacterByIdAsync(character.Id);

        var updatedState = result.States.First();

        Assert.Equal("updated_state", updatedState.StateName);
        Assert.Equal("updated_desc", updatedState.Description);
    }
    
    [Fact]
    public async Task TestCreateCharacterWithStatesAsync()
    {
        var character = CreateCharacter();

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);
        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
                              VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var states = GetCharacterStates(new[] { "idle", "angry" }, character, image, transform);
        character.States = states;

        await repo.AddOrUpdateFullAsync(character);

        var result = await repo.GetFullCharacterByIdAsync(character.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.States.Count);
    }
    
    [Fact]
    public async Task TestGetByIdCharacterWithStatesAsync()
    {
        var character = CreateCharacter();
        await repo.AddAsync(character);

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        var sql = @"INSERT INTO ""CharacterStates"" (id, character_id, image_id, state_name, transform_id)
            VALUES (@Id, @CharacterId, @ImageId, @StateName, @TransformId)";

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);
        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
                              VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var states = GetCharacterStates(["idle", "angry"], character, image, transform);

        foreach (var state in states)
            await conn.ExecuteAsync(sql, state);

        var result = await repo.GetFullCharacterByIdAsync(character.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.States.Count);
    }
    
    [Fact]
    public async Task TestUpdateCharacterWithStatesAsync()
    {
        var character = CreateCharacter();

        var image = CreateImage(novelId);
        var transform = CreateTransform();

        await using var conn = new NpgsqlConnection(connectionString);

        await conn.ExecuteAsync(@"INSERT INTO ""Images"" 
        VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)", image);

        await conn.ExecuteAsync(@"INSERT INTO ""Transforms"" 
        (id, height, width, scale, x_pos, y_pos, rotation, z_index) 
        VALUES (@Id, @Height, @Width, @Scale, @XPos, @YPos, 0, 0)", transform);

        var states = GetCharacterStates(
            new[] { "idle", "angry" },
            character,
            image,
            transform
        );

        character.States = states;

        await repo.AddOrUpdateFullAsync(character);
        
        character.Name = "Updated Character";
        states[0].StateName = "idle_updated";
        
        var newState = CreateCharacterState(character, image, transform);
        newState.StateName = "new_state";

        character.States.Add(newState);

        await repo.AddOrUpdateFullAsync(character);

        var result = await repo.GetFullCharacterByIdAsync(character.Id);

        Assert.NotNull(result);
        Assert.Equal("Updated Character", result.Name);

        Assert.Equal(3, result.States.Count);

        Assert.Contains(result.States, s => s.StateName == "idle_updated");
        Assert.Contains(result.States, s => s.StateName == "angry");
        Assert.Contains(result.States, s => s.StateName == "new_state");
    }
    
    // [Fact]  //доделаю после тестирования степов
    // public async Task TestGetStepCharacterAsyncById()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // [Fact]
    // public async Task TestCreateStepCharacterAsync()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // [Fact]
    // public async Task TestDeleteStepCharacterAsync()
    // {
    //     throw new NotImplementedException();
    // }

    public async Task DisposeAsync()
    {
        await DeleteFromTableAsync("Novels", [novelId]);
        await DeleteFromTableAsync("Characters", characterIds);
        await DeleteFromTableAsync("CharacterStates", statesIds);
        await DeleteFromTableAsync("Images", imagesIds);
        await DeleteFromTableAsync("Transforms", transformsIds);
    }
}