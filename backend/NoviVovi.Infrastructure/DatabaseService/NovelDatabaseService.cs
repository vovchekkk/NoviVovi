using System.Data;
using Dapper;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using Npgsql;

namespace NoviVovi.Infrastructure.DatabaseService;

public class NovelDatabaseService : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public NovelDatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
        }
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
    
    // ==================== БАЗОВЫЕ GET-МЕТОДЫ (плоские) ====================

    public async Task<NovelDbO?> GetNovelByIdAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
            FROM ""Novels""
            WHERE id = @Id";

        return await conn.QueryFirstOrDefaultAsync<NovelDbO>(sql, new { Id = id });
    }

    private async Task<IEnumerable<NovelDbO>> GetPublicNovelsByUserAsync(Guid userId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
            FROM ""Novels""
            WHERE is_public = true
            ORDER BY created_at DESC";

        return await conn.QueryAsync<NovelDbO>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<NovelDbO>> GetAllNovelsAsync(bool onlyPublic = true)
    {
        await using var conn = await GetConnectionAsync();
        
        var sql = @"
            SELECT 
                id AS Id,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
            FROM ""Novels""";
        
        if (onlyPublic)
        {
            sql += " WHERE is_public = true";
        }
        
        sql += " ORDER BY created_at DESC";
        
        return await conn.QueryAsync<NovelDbO>(sql);
    }

    public async Task<IEnumerable<LabelDbO>> GetLabelsByNovelIdAsync(Guid novelId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE novel_id = @NovelId
            ORDER BY label_name";

        return await conn.QueryAsync<LabelDbO>(sql, new { NovelId = novelId });
    }

    public async Task<IEnumerable<StepDbO>> GetStepsByLabelIdAsync(Guid labelId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                label_id AS LabelId,
                replica_id AS ReplicaId,
                menu_id AS MenuId,
                bg_id AS BgId,
                next_label_id AS NextLabelId,
                step_order AS StepOrder,
                step_type AS StepType
            FROM ""Steps""
            WHERE label_id = @LabelId
            ORDER BY step_order";

        return await conn.QueryAsync<StepDbO>(sql, new { LabelId = labelId });
    }

    public async Task<ReplicaDbO?> GetReplicaByIdAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                speaker_id AS SpeakerId,
                text AS Text
            FROM ""Replicas""
            WHERE id = @Id";

        return await conn.QueryFirstOrDefaultAsync<ReplicaDbO>(sql, new { Id = id });
    }

    private async Task<IEnumerable<CharacterDbO>> GetCharactersByNovelIdAsync(Guid novelId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                name AS Name,
                name_color AS NameColor,
                description AS Description
            FROM ""Characters""
            WHERE novel_id = @NovelId
            ORDER BY name";

        return await conn.QueryAsync<CharacterDbO>(sql, new { NovelId = novelId });
    }

    private async Task<IEnumerable<CharacterStateDbO>> GetCharacterStatesByCharacterIdAsync(Guid characterId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                character_id AS CharacterId,
                image_id AS ImageId,
                state_name AS StateName,
                description AS Description
            FROM ""CharacterStates""
            WHERE character_id = @CharacterId
            ORDER BY state_name";

        return await conn.QueryAsync<CharacterStateDbO>(sql, new { CharacterId = characterId });
    }

    public async Task<IEnumerable<ImageDbO>> GetImagesByNovelIdAsync(Guid novelId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                name AS Name,
                ""URL"" AS Url,
                format AS Format,
                img_type AS ImgType,
                height AS Height,
                width AS Width,
                size AS Size
            FROM ""Images""
            WHERE novel_id = @NovelId
            ORDER BY name";

        return await conn.QueryAsync<ImageDbO>(sql, new { NovelId = novelId });
    }
    

    public async Task<TransformDbO?> GetTransformByIdAsync(Guid transformId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                scale AS Scale,
                rotation AS Rotation,
                z_index AS ZIndex,
                width AS Width,
                height AS Height,
                x_pos AS XPos,
                y_pos AS YPos
            FROM ""Transforms""
            WHERE id = @Id";

        return await conn.QueryFirstOrDefaultAsync<TransformDbO>(sql, new { Id = transformId });
    }

    public async Task<BackgroundDbO?> GetBackgroundByIdAsync(Guid bgId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                img AS Img,
                transform_id AS TransformId
            FROM ""Backgrounds""
            WHERE id = @Id";

        return await conn.QueryFirstOrDefaultAsync<BackgroundDbO>(sql, new { Id = bgId });
    }

    // ==================== ПОЛНЫЕ МЕТОДЫ (с вложенными объектами) ====================

    /// <summary>
    /// Получить меню с выборами (полностью)
    /// </summary>
    public async Task<MenuDbO?> GetFullMenuByIdAsync(Guid menuId)
    {
        await using var conn = await GetConnectionAsync();
    
        const string menuSql = @"
        SELECT 
            id AS Id,
            name AS Name,
            text AS Text,
            description AS Description
        FROM ""Menus""
        WHERE id = @MenuId";

        var menu = await conn.QueryFirstOrDefaultAsync<MenuDbO>(menuSql, new { MenuId = menuId });
    
        if (menu == null)
            return null;

        const string choicesSql = @"
        SELECT 
            id AS Id,
            menu_id AS MenuId,
            next_label_id AS NextLabelId,
            name AS Name,
            text AS Text
        FROM ""Choices""
        WHERE menu_id = @MenuId
        ORDER BY name";

        var choices = await conn.QueryAsync<ChoiceDbO>(choicesSql, new { MenuId = menuId });
        
        foreach (var choice in choices)
        {
            const string labelSql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                label_name AS LabelName
            FROM ""Labels""
            WHERE id = @LabelId";
        
            choice.NextLabel = await conn.QueryFirstOrDefaultAsync<LabelDbO>(
                labelSql, 
                new { LabelId = choice.NextLabelId }
            );
        }
    
        menu.Choices = choices.AsList();
        return menu;
    }

    /// <summary>
    /// Получить состояние персонажа с изображением
    /// </summary>
    public async Task<CharacterStateDbO?> GetFullCharacterStateByIdAsync(Guid stateId)
    {
        await using var conn = await GetConnectionAsync();
    
        const string sql = @"
        SELECT 
            id AS Id,
            character_id AS CharacterId,
            image_id AS ImageId,
            state_name AS StateName,
            description AS Description,
            transform_id AS TransformId
        FROM ""CharacterStates""
        WHERE id = @StateId";

        var state = await conn.QueryFirstOrDefaultAsync<CharacterStateDbO>(sql, new { StateId = stateId });
    
        if (state == null)
            return null;

        // Загружаем изображение
        const string imageSql = @"
        SELECT 
            id AS Id,
            novel_id AS NovelId,
            name AS Name,
            ""URL"" AS Url,
            format AS Format,
            img_type AS ImgType,
            height AS Height,
            width AS Width,
            size AS Size
        FROM ""Images""
        WHERE id = @ImageId";

        state.Image = await conn.QueryFirstOrDefaultAsync<ImageDbO>(imageSql, new { ImageId = state.ImageId });
    
        // Загружаем трансформацию (если есть)
        if (state.TransformId.HasValue)
        {
            state.Transform = await GetTransformByIdAsync(state.TransformId.Value);
        }

        return state;
    }

    /// <summary>
    /// Получить всех персонажей новеллы с их состояниями
    /// </summary>
    public async Task<IEnumerable<CharacterDbO>> GetFullCharactersByNovelIdAsync(Guid novelId)
    {
        await using var conn = await GetConnectionAsync();
        
        var characters = (await GetCharactersByNovelIdAsync(novelId)).ToList();
        
        foreach (var character in characters)
        {
            var states = await GetCharacterStatesByCharacterIdAsync(character.Id);
            
            // Загружаем изображения для каждого состояния
            foreach (var state in states)
            {
                const string imageSql = @"
                    SELECT 
                        id AS Id,
                        novel_id AS NovelId,
                        name AS Name,
                        ""URL"" AS Url,
                        format AS Format,
                        img_type AS ImgType,
                        height AS Height,
                        width AS Width,
                        size AS Size
                    FROM ""Images""
                    WHERE id = @ImageId";
                
                state.Image = await conn.QueryFirstOrDefaultAsync<ImageDbO>(imageSql, new { ImageId = state.ImageId });
            }
            
            character.States = states.ToList();
        }
        
        return characters;
    }

    /// <summary>
    /// Получить полный шаг с репликой, меню, фоном и персонажами
    /// </summary>
    public async Task<StepDbO?> GetFullStepByIdAsync(Guid stepId)
    {
        await using var conn = await GetConnectionAsync();
        
        const string stepSql = @"
            SELECT 
                id AS Id,
                label_id AS LabelId,
                replica_id AS ReplicaId,
                menu_id AS MenuId,
                bg_id AS BgId,
                next_label_id AS NextLabelId,
                step_order AS StepOrder,
                step_type AS StepType
            FROM ""Steps""
            WHERE id = @StepId";

        var step = await conn.QueryFirstOrDefaultAsync<StepDbO>(stepSql, new { StepId = stepId });
        
        if (step == null)
            return null;

        // Загружаем реплику
        if (step.ReplicaId.HasValue)
        {
            step.Replica = await GetReplicaByIdAsync(step.ReplicaId.Value);
        }

        // Загружаем меню с выборами
        if (step.MenuId.HasValue)
        {
            step.Menu = await GetFullMenuByIdAsync(step.MenuId.Value);
        }

        // Загружаем следующий лейбл (для навигации)
        if (step.NextLabelId.HasValue)
        {
            const string labelSql = @"
                SELECT 
                    id AS Id,
                    novel_id AS NovelId,
                    label_name AS LabelName
                FROM ""Labels""
                WHERE id = @Id";
            
            step.NextLabel = await conn.QueryFirstOrDefaultAsync<LabelDbO>(labelSql, new { Id = step.NextLabelId });
        }

        // Загружаем фон
        // Загружаем фон
        if (step.BgId.HasValue)
        {
            step.Background = await GetBackgroundByIdAsync(step.BgId.Value);
    
            // Загружаем изображение для фона
            if (step.Background != null)
            {
                const string imageSql = @"
            SELECT 
                id AS Id,
                novel_id AS NovelId,
                name AS Name,
                ""URL"" AS Url,
                format AS Format,
                img_type AS ImgType,
                height AS Height,
                width AS Width,
                size AS Size
            FROM ""Images""
            WHERE id = @ImageId";
        
                step.Background.Image = await conn.QueryFirstOrDefaultAsync<ImageDbO>(
                    imageSql, 
                    new { ImageId = step.Background.Img }
                );
        
                // Загружаем трансформацию для фона
                if (step.Background.TransformId.HasValue)
                {
                    step.Background.Transform = await GetTransformByIdAsync(step.Background.TransformId.Value);
                }
            }
        }

        // Загружаем персонажей на шаге
        const string stepCharactersSql = @"
            SELECT 
                id AS Id,
                transform_id AS TransformId,
                character_state_id AS CharacterStateId,
                step_id AS StepId
            FROM ""StepCharacter""
            WHERE step_id = @StepId";

        var stepCharacters = await conn.QueryAsync<StepCharacterDbO>(stepCharactersSql, new { StepId = stepId });
        
        foreach (var sc in stepCharacters)
        {
            // Загружаем трансформацию
            if (sc.TransformId.HasValue)
            {
                sc.Transform = await GetTransformByIdAsync(sc.TransformId.Value);
            }
            
            // Загружаем состояние персонажа с изображением
            sc.CharacterState = await GetFullCharacterStateByIdAsync(sc.CharacterStateId);
        }
        
        step.StepCharacters = stepCharacters.AsList();

        return step;
    }

    /// <summary>
    /// Получить полную новеллу (со всеми зависимостями)
    /// </summary>
    public async Task<NovelDbO?> GetFullNovelByIdAsync(Guid novelId)
    {
        await using var conn = await GetConnectionAsync();
    
        var novel = await GetNovelByIdAsync(novelId);
    
        if (novel == null)
            return null;
        
        var labels = (await GetLabelsByNovelIdAsync(novelId)).ToList();
        foreach (var label in labels)
        {
            var steps = await GetStepsByLabelIdAsync(label.Id);
            var fullSteps = new List<StepDbO>();
        
            foreach (var step in steps)
            {
                var fullStep = await GetFullStepByIdAsync(step.Id);
                if (fullStep != null)
                    fullSteps.Add(fullStep);
            }
        
            label.Steps = fullSteps;
        }
    
        novel.Labels = labels;

        return novel;
    }

    // ==================== CRUD МЕТОДЫ ====================
    
    public async Task<Guid> CreateNovelAsync(NovelDbO novel)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Novels"" (
                id, title, description, 
                start_label_id, cover_image_id, is_public, 
                created_at, edited_at
            ) VALUES (
                @Id, @Title, @Description, 
                @StartLabelId, @CoverImageId, @IsPublic, 
                @CreatedAt, @EditedAt
            )";

        await conn.ExecuteAsync(sql, novel);
        return novel.Id;
    }

    public async Task UpdateNovelAsync(NovelDbO novel)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Novels"" SET
                title = @Title,
                description = @Description,
                start_label_id = @StartLabelId,
                cover_image_id = @CoverImageId,
                is_public = @IsPublic,
                edited_at = @EditedAt
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, novel);
    }

    public async Task DeleteNovelAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Novels\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateLabelAsync(LabelDbO label)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Labels"" (id, novel_id, label_name)
            VALUES (@Id, @NovelId, @LabelName)";

        await conn.ExecuteAsync(sql, label);
        return label.Id;
    }
    
    public async Task UpdateLabelAsync(LabelDbO label)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Labels"" SET
                novel_id = @NovelId,
                label_name = @LabelName
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, label);
    }

    public async Task DeleteLabelAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Labels\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateStepAsync(StepDbO step)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Steps"" (
                id, label_id, replica_id, menu_id, bg_id, 
                next_label_id, step_order, step_type
            ) VALUES (
                @Id, @LabelId, @ReplicaId, @MenuId, @BgId, 
                @NextLabelId, @StepOrder, @StepType
            )";

        await conn.ExecuteAsync(sql, step);
        return step.Id;
    }

    public async Task UpdateStepAsync(StepDbO step)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Steps"" SET
                label_id = @LabelId,
                replica_id = @ReplicaId,
                menu_id = @MenuId,
                bg_id = @BgId,
                next_label_id = @NextLabelId,
                step_order = @StepOrder,
                step_type = @StepType
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, step);
    }

    public async Task DeleteStepAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Steps\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateReplicaAsync(ReplicaDbO replica)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Replicas"" (id, speaker_id, text)
            VALUES (@Id, @SpeakerId, @Text)";

        await conn.ExecuteAsync(sql, replica);
        return replica.Id;
    }

    public async Task UpdateReplicaAsync(ReplicaDbO replica)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Replicas"" SET
                speaker_id = @SpeakerId,
                text = @Text
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, replica);
    }

    public async Task DeleteReplicaAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Replicas\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateCharacterAsync(CharacterDbO character)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Characters"" (id, novel_id, name, name_color, description)
            VALUES (@Id, @NovelId, @Name, @NameColor, @Description)";

        await conn.ExecuteAsync(sql, character);
        return character.Id;
    }

    public async Task UpdateCharacterAsync(CharacterDbO character)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Characters"" SET
                novel_id = @NovelId,
                name = @Name,
                name_color = @NameColor,
                description = @Description
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, character);
    }

    public async Task DeleteCharacterAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Characters\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateCharacterStateAsync(CharacterStateDbO state)
    {
        await using var conn = await GetConnectionAsync();
    
        const string sql = @"
        INSERT INTO ""CharacterStates"" (id, character_id, image_id, state_name, description, transform_id)
        VALUES (@Id, @CharacterId, @ImageId, @StateName, @Description, @TransformId)";

        await conn.ExecuteAsync(sql, state);
        return state.Id;
    }

    public async Task UpdateCharacterStateAsync(CharacterStateDbO state)
    {
        await using var conn = await GetConnectionAsync();
    
        const string sql = @"
        UPDATE ""CharacterStates"" SET
            character_id = @CharacterId,
            image_id = @ImageId,
            state_name = @StateName,
            description = @Description,
            transform_id = @TransformId
        WHERE id = @Id";

        await conn.ExecuteAsync(sql, state);
    }

    public async Task DeleteCharacterStateAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"CharacterStates\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateImageAsync(ImageDbO image)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Images"" (id, novel_id, name, ""URL"", format, img_type, height, width, size)
            VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)";

        await conn.ExecuteAsync(sql, image);
        return image.Id;
    }

    public async Task UpdateImageAsync(ImageDbO image)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Images"" SET
                novel_id = @NovelId,
                name = @Name,
                ""URL"" = @Url,
                format = @Format,
                img_type = @ImgType,
                height = @Height,
                width = @Width,
                size = @Size
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, image);
    }

    public async Task DeleteImageAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Images\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateMenuAsync(MenuDbO menu)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Menus"" (id, name, text, description)
            VALUES (@Id, @Name, @Text, @Description)";

        await conn.ExecuteAsync(sql, menu);
        return menu.Id;
    }

    public async Task<Guid> CreateChoiceAsync(ChoiceDbO choice)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Choices"" (id, menu_id, next_label_id, name, text)
            VALUES (@Id, @MenuId, @NextLabelId, @Name, @Text)";

        await conn.ExecuteAsync(sql, choice);
        return choice.Id;
    }

    public async Task<Guid> CreateStepCharacterAsync(StepCharacterDbO stepCharacter)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""StepCharacter"" (id, transform_id, character_state_id, step_id)
            VALUES (@Id, @TransformId, @CharacterStateId, @StepId)";

        await conn.ExecuteAsync(sql, stepCharacter);
        return stepCharacter.Id;
    }

    public async Task DeleteStepCharacterAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"StepCharacter\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid> CreateBackgroundAsync(BackgroundDbO background)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Backgrounds"" (id, img, transform_id)
            VALUES (@Id, @Img, @TransformId)";

        await conn.ExecuteAsync(sql, background);
        return background.Id;
    }

    public async Task<Guid> CreateTransformAsync(TransformDbO transform)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Transforms"" (id, scale, rotation, z_index, width, height, x_pos, y_pos)
            VALUES (@Id, @Scale, @Rotation, @ZIndex, @Width, @Height, @XPos, @YPos)";

        await conn.ExecuteAsync(sql, transform);
        return transform.Id;
    }
    
}