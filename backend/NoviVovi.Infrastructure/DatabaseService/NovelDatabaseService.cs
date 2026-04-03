using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;

namespace NoviVovi.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

/*EntityFramework боль в дырка задница из-за циклических ссылок*/

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
    
    public async Task<NovelDbO?> GetNovelByIdAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                user_id AS UserId,
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

    /// <summary>
    /// Получить все публичные новеллы пользователя
    /// </summary>
    public async Task<IEnumerable<NovelDbO>> GetPublicNovelsByUserAsync(Guid userId)
    {
        using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                user_id AS UserId,
                title AS Title,
                description AS Description,
                start_label_id AS StartLabelId,
                cover_image_id AS CoverImageId,
                is_public AS IsPublic,
                created_at AS CreatedAt,
                edited_at AS EditedAt
            FROM ""Novels""
            WHERE user_id = @UserId AND is_public = true
            ORDER BY created_at DESC";

        return await conn.QueryAsync<NovelDbO>(sql, new { UserId = userId });
    }

    /// <summary>
    /// Получить все новеллы (с учётом приватных по коду)
    /// </summary>
    public async Task<IEnumerable<NovelDbO>> GetAllNovelsAsync(bool onlyPublic = true)
    {
        using var conn = await GetConnectionAsync();
        
        var sql = @"
            SELECT 
                id AS Id,
                user_id AS UserId,
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

    /// <summary>
    /// Получить все метки (сцены) новеллы
    /// </summary>
    public async Task<IEnumerable<LabelDbO>> GetLabelsByNovelIdAsync(Guid novelId)
    {
        using var conn = await GetConnectionAsync();
        
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

    /// <summary>
    /// Получить все шаги метки (сцены)
    /// </summary>
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

    /// <summary>
    /// Получить реплику по ID
    /// </summary>
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

    /// <summary>
    /// Получить персонажей новеллы
    /// </summary>
    public async Task<IEnumerable<CharacterDbO>> GetCharactersByNovelIdAsync(Guid novelId)
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

    /// <summary>
    /// Получить состояния персонажа
    /// </summary>
    public async Task<IEnumerable<CharacterStateDbO>> GetCharacterStatesByCharacterIdAsync(Guid characterId)
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

    /// <summary>
    /// Получить меню с выборами
    /// </summary>
    public async Task<FullMenuDbO?> GetFullMenuByIdAsync(Guid menuId)
    {
        await using var conn = await GetConnectionAsync();
        
        // Получаем меню
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

        // Получаем выборы
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

        return new FullMenuDbO
        {
            Menu = menu,
            Choices = choices.ToList()
        };
    }

    /// <summary>
    /// Получить полный шаг с репликой, меню и персонажами
    /// </summary>
    public async Task<FullStepDbO?> GetFullStepByIdAsync(Guid stepId)
    {
        await using var conn = await GetConnectionAsync();
        
        // Получаем шаг
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

        var fullStep = new FullStepDbO
        {
            Step = step
        };

        // Получаем реплику если есть
        if (step.ReplicaId.HasValue)
        {
            fullStep.Replica = await GetReplicaByIdAsync(step.ReplicaId.Value);
        }

        // Получаем меню если есть
        if (step.MenuId.HasValue)
        {
            fullStep.Menu = await GetFullMenuByIdAsync(step.MenuId.Value);
        }

        // Получаем фон если есть
        if (step.BgId.HasValue)
        {
            const string bgSql = @"
                SELECT 
                    id AS Id,
                    img AS Img,
                    transform_id AS TransformId
                FROM ""Backgrounds""
                WHERE id = @BgId";

            fullStep.Background = await conn.QueryFirstOrDefaultAsync<BackgroundDbO>(bgSql, new { BgId = step.BgId });
        }

        // Получаем персонажей на шаге
        const string stepCharactersSql = @"
            SELECT 
                id AS Id,
                transform_id AS TransformId,
                character_state_id AS CharacterStateId,
                step_id AS StepId
            FROM ""StepCharacter""
            WHERE step_id = @StepId";

        fullStep.StepCharacters = (await conn.QueryAsync<StepCharacterDbO>(stepCharactersSql, new { StepId = stepId })).ToList();

        return fullStep;
    }

    /// <summary>
    /// Получить полную новеллу (со всеми зависимостями)
    /// </summary>
    public async Task<FullNovelDbO?> GetFullNovelByIdAsync(Guid novelId)
    {
        using var conn = await GetConnectionAsync();
        
        var novel = await GetNovelByIdAsync(novelId);
        
        if (novel == null)
            return null;

        var fullNovel = new FullNovelDbO
        {
            Novel = novel,
            Labels = (await GetLabelsByNovelIdAsync(novelId)).ToList(),
            Characters = (await GetCharactersByNovelIdAsync(novelId)).ToList(),
            Images = (await GetImagesByNovelIdAsync(novelId)).ToList()
        };

        return fullNovel;
    }

    /// <summary>
    /// Получить изображения новеллы
    /// </summary>
    public async Task<IEnumerable<ImageDbO>> GetImagesByNovelIdAsync(Guid novelId)
    {
        using var conn = await GetConnectionAsync();
        
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

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    public async Task<UserDbO?> GetUserByIdAsync(Guid userId)
    {
        using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                private_code AS PrivateCode
            FROM ""Users""
            WHERE id = @UserId";

        return await conn.QueryFirstOrDefaultAsync<UserDbO>(sql, new { UserId = userId });
    }

    /// <summary>
    /// Получить пользователя по приватному коду
    /// </summary>
    public async Task<UserDbO?> GetUserByPrivateCodeAsync(string privateCode)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                private_code AS PrivateCode
            FROM ""Users""
            WHERE private_code = @PrivateCode";

        return await conn.QueryFirstOrDefaultAsync<UserDbO>(sql, new { PrivateCode = privateCode });
    }


    //-----------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Создать новеллу
    /// </summary>
    public async Task<Guid> CreateNovelAsync(NovelDbO novel)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Novels"" (
                id, user_id, title, description, 
                start_label_id, cover_image_id, is_public, 
                created_at, edited_at
            ) VALUES (
                @Id, @UserId, @Title, @Description, 
                @StartLabelId, @CoverImageId, @IsPublic, 
                @CreatedAt, @EditedAt
            )";

        await conn.ExecuteAsync(sql, novel);
        return novel.Id;
    }

    /// <summary>
    /// Обновить новеллу
    /// </summary>
    public async Task UpdateNovelAsync(NovelDbO novel)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""Novels"" SET
                user_id = @UserId,
                title = @Title,
                description = @Description,
                start_label_id = @StartLabelId,
                cover_image_id = @CoverImageId,
                is_public = @IsPublic,
                edited_at = @EditedAt
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, novel);
    }

    /// <summary>
    /// Удалить новеллу
    /// </summary>
    public async Task DeleteNovelAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Novels\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать метку (сцену)
    /// </summary>
    public async Task<Guid> CreateLabelAsync(LabelDbO label)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Labels"" (id, novel_id, label_name)
            VALUES (@Id, @NovelId, @LabelName)";

        await conn.ExecuteAsync(sql, label);
        return label.Id;
    }
    
    /// <summary>
    /// Обновить метку
    /// </summary>
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

    /// <summary>
    /// Удалить метку
    /// </summary>
    public async Task DeleteLabelAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Labels\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать шаг
    /// </summary>
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

    /// <summary>
    /// Обновить шаг
    /// </summary>
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

    /// <summary>
    /// Удалить шаг
    /// </summary>
    public async Task DeleteStepAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Steps\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать реплику
    /// </summary>
    public async Task<Guid> CreateReplicaAsync(ReplicaDbO replica)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Replicas"" (id, speaker_id, text)
            VALUES (@Id, @SpeakerId, @Text)";

        await conn.ExecuteAsync(sql, replica);
        return replica.Id;
    }

    /// <summary>
    /// Обновить реплику
    /// </summary>
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

    /// <summary>
    /// Удалить реплику
    /// </summary>
    public async Task DeleteReplicaAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Replicas\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать персонажа
    /// </summary>
    public async Task<Guid> CreateCharacterAsync(CharacterDbO character)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Characters"" (id, novel_id, name, name_color, description)
            VALUES (@Id, @NovelId, @Name, @NameColor, @Description)";

        await conn.ExecuteAsync(sql, character);
        return character.Id;
    }

    /// <summary>
    /// Обновить персонажа
    /// </summary>
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

    /// <summary>
    /// Удалить персонажа
    /// </summary>
    public async Task DeleteCharacterAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Characters\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать состояние персонажа
    /// </summary>
    public async Task<Guid> CreateCharacterStateAsync(CharacterStateDbO state)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""CharacterStates"" (id, character_id, image_id, state_name, description)
            VALUES (@Id, @CharacterId, @ImageId, @StateName, @Description)";

        await conn.ExecuteAsync(sql, state);
        return state.Id;
    }

    /// <summary>
    /// Обновить состояние персонажа
    /// </summary>
    public async Task UpdateCharacterStateAsync(CharacterStateDbO state)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            UPDATE ""CharacterStates"" SET
                character_id = @CharacterId,
                image_id = @ImageId,
                state_name = @StateName,
                description = @Description
            WHERE id = @Id";

        await conn.ExecuteAsync(sql, state);
    }

    /// <summary>
    /// Удалить состояние персонажа
    /// </summary>
    public async Task DeleteCharacterStateAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"CharacterStates\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать изображение
    /// </summary>
    public async Task<Guid> CreateImageAsync(ImageDbO image)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Images"" (id, novel_id, name, ""URL"", format, img_type, height, width, size)
            VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)";

        await conn.ExecuteAsync(sql, image);
        return image.Id;
    }

    /// <summary>
    /// Обновить изображение
    /// </summary>
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

    /// <summary>
    /// Удалить изображение
    /// </summary>
    public async Task DeleteImageAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"Images\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать меню
    /// </summary>
    public async Task<Guid> CreateMenuAsync(MenuDbO menu)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Menus"" (id, name, text, description)
            VALUES (@Id, @Name, @Text, @Description)";

        await conn.ExecuteAsync(sql, menu);
        return menu.Id;
    }

    /// <summary>
    /// Создать выбор
    /// </summary>
    public async Task<Guid> CreateChoiceAsync(ChoiceDbO choice)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Choices"" (id, menu_id, next_label_id, name, text)
            VALUES (@Id, @MenuId, @NextLabelId, @Name, @Text)";

        await conn.ExecuteAsync(sql, choice);
        return choice.Id;
    }

    /// <summary>
    /// Создать связь шага с персонажем
    /// </summary>
    public async Task<Guid> CreateStepCharacterAsync(StepCharacterDbO stepCharacter)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""StepCharacter"" (id, transform_id, character_state_id, step_id)
            VALUES (@Id, @TransformId, @CharacterStateId, @StepId)";

        await conn.ExecuteAsync(sql, stepCharacter);
        return stepCharacter.Id;
    }

    /// <summary>
    /// Удалить связь шага с персонажем
    /// </summary>
    public async Task DeleteStepCharacterAsync(Guid id)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = "DELETE FROM \"StepCharacter\" WHERE id = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Создать фон
    /// </summary>
    public async Task<Guid> CreateBackgroundAsync(BackgroundDbO background)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Backgrounds"" (id, img, transform_id)
            VALUES (@Id, @Img, @TransformId)";

        await conn.ExecuteAsync(sql, background);
        return background.Id;
    }

    /// <summary>
    /// Создать трансформацию
    /// </summary>
    public async Task<Guid> CreateTransformAsync(TransformDbO transform)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Transforms"" (id, scale, rotation, z_index, width, height, x_pos, y_pos)
            VALUES (@Id, @Scale, @Rotation, @ZIndex, @Width, @Height, @XPos, @YPos)";

        await conn.ExecuteAsync(sql, transform);
        return transform.Id;
    }

    /// <summary>
    /// Создать пользователя
    /// </summary>
    public async Task<Guid> CreateUserAsync(UserDbO user)
    {
        await using var conn = await GetConnectionAsync();
        
        const string sql = @"
            INSERT INTO ""Users"" (id, private_code)
            VALUES (@Id, @PrivateCode)";

        await conn.ExecuteAsync(sql, user);
        return user.Id;
    }

}