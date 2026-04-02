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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
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
        using var conn = await GetConnectionAsync();
        
        const string sql = @"
            SELECT 
                id AS Id,
                private_code AS PrivateCode
            FROM ""Users""
            WHERE private_code = @PrivateCode";

        return await conn.QueryFirstOrDefaultAsync<UserDbO>(sql, new { PrivateCode = privateCode });
    }
}