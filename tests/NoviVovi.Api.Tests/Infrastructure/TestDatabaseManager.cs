using System.Data;
using Dapper;
using Npgsql;

namespace NoviVovi.Api.Tests.Infrastructure;

/// <summary>
/// Manages test database lifecycle: creation, schema setup, and cleanup.
/// Each test gets a fresh isolated database.
/// </summary>
public class TestDatabaseManager : IDisposable
{
    private const string MasterConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=true";
    private readonly string _testDatabaseName;
    private readonly string _testConnectionString;
    private bool _disposed;

    public TestDatabaseManager()
    {
        _testDatabaseName = $"test_novels_{Guid.NewGuid():N}";
        _testConnectionString = $"Host=localhost;Port=5432;Database={_testDatabaseName};Username=postgres;Password=postgres;Include Error Detail=true";
    }

    public string ConnectionString => _testConnectionString;

    /// <summary>
    /// Creates test database and applies schema.
    /// </summary>
    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();
        await ApplySchemaAsync();
    }

    private async Task CreateDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(MasterConnectionString);
        await connection.OpenAsync();
        
        await connection.ExecuteAsync($@"CREATE DATABASE ""{_testDatabaseName}""");
    }

    private async Task ApplySchemaAsync()
    {
        await using var connection = new NpgsqlConnection(_testConnectionString);
        await connection.OpenAsync();

        var schema = GetDatabaseSchema();
        await connection.ExecuteAsync(schema);
    }

    /// <summary>
    /// Cleans up test database.
    /// </summary>
    public async Task CleanupAsync()
    {
        if (_disposed)
            return;

        try
        {
            await using var connection = new NpgsqlConnection(MasterConnectionString);
            await connection.OpenAsync();

            // Terminate all connections to test database
            await connection.ExecuteAsync($@"
                SELECT pg_terminate_backend(pg_stat_activity.pid)
                FROM pg_stat_activity
                WHERE pg_stat_activity.datname = '{_testDatabaseName}'
                  AND pid <> pg_backend_pid();
            ");

            // Drop database
            await connection.ExecuteAsync($@"DROP DATABASE IF EXISTS ""{_testDatabaseName}""");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to cleanup test database {_testDatabaseName}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        CleanupAsync().GetAwaiter().GetResult();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private static string GetDatabaseSchema()
    {
        return @"
-- =========================
-- NOVELS
-- =========================
CREATE TABLE ""Novels"" (
    ""id"" UUID PRIMARY KEY,
    ""title"" VARCHAR(255) NOT NULL,
    ""description"" TEXT,
    ""start_label_id"" UUID,
    ""cover_image_id"" UUID,
    ""is_public"" BOOLEAN NOT NULL,
    ""created_at"" TIMESTAMP with TIME ZONE DEFAULT NOW() NOT NULL,
    ""edited_at"" TIMESTAMP with TIME ZONE DEFAULT NOW() NOT NULL
);

-- =========================
-- IMAGES
-- =========================
CREATE TABLE ""Images"" (
    ""id"" UUID PRIMARY KEY,
    ""novel_id"" UUID NOT NULL,
    ""name"" VARCHAR(100) NOT NULL,
    ""url"" TEXT NOT NULL,
    ""format"" VARCHAR(20) NOT NULL,
    ""img_type"" VARCHAR(50) NOT NULL,
    ""height"" INT NOT NULL,
    ""width"" INT NOT NULL,
    ""size"" INT NOT NULL
);

-- =========================
-- TRANSFORMS
-- =========================
CREATE TABLE ""Transforms"" (
    ""id"" UUID PRIMARY KEY,
    ""scale"" DECIMAL NOT NULL DEFAULT 1,
    ""rotation"" DECIMAL NOT NULL DEFAULT 0,
    ""z_index"" INTEGER NOT NULL DEFAULT 0,
    ""width"" INTEGER NOT NULL,
    ""height"" INTEGER NOT NULL,
    ""x_pos"" DECIMAL NOT NULL DEFAULT 0,
    ""y_pos"" DECIMAL NOT NULL DEFAULT 0
);

-- =========================
-- CHARACTERS
-- =========================
CREATE TABLE ""Characters"" (
    ""id"" UUID PRIMARY KEY,
    ""novel_id"" UUID NOT NULL,
    ""name"" VARCHAR(100) NOT NULL,
    ""name_color"" CHAR(6) NOT NULL,
    ""description"" TEXT
);

-- =========================
-- CHARACTER STATES
-- =========================
CREATE TABLE ""CharacterStates"" (
    ""id"" UUID PRIMARY KEY,
    ""character_id"" UUID NOT NULL,
    ""image_id"" UUID NOT NULL,
    ""state_name"" VARCHAR(100) NOT NULL,
    ""description"" TEXT,
    ""transform_id"" UUID
);

-- =========================
-- LABELS
-- =========================
CREATE TABLE ""Labels"" (
    ""id"" UUID PRIMARY KEY,
    ""novel_id"" UUID NOT NULL,
    ""label_name"" VARCHAR(100) NOT NULL
);

-- =========================
-- MENUS
-- =========================
CREATE TABLE ""Menus"" (
    ""id"" UUID PRIMARY KEY
);

-- =========================
-- CHOICES
-- =========================
CREATE TABLE ""Choices"" (
    ""id"" UUID PRIMARY KEY,
    ""menu_id"" UUID NOT NULL,
    ""next_label_id"" UUID,
    ""text"" TEXT
);

-- =========================
-- BACKGROUNDS
-- =========================
CREATE TABLE ""Backgrounds"" (
    ""id"" UUID PRIMARY KEY,
    ""img"" UUID NOT NULL,
    ""transform_id"" UUID
);

-- =========================
-- REPLICAS
-- =========================
CREATE TABLE ""Replicas"" (
    ""id"" UUID PRIMARY KEY,
    ""speaker_id"" UUID,
    ""text"" TEXT
);

-- =========================
-- STEPS
-- =========================
CREATE TABLE ""Steps"" (
    ""id"" UUID PRIMARY KEY,
    ""label_id"" UUID NOT NULL,
    ""replica_id"" UUID,
    ""menu_id"" UUID,
    ""background_id"" UUID,
    ""character_id"" UUID,
    ""next_label_id"" UUID,
    ""step_order"" INT NOT NULL,
    ""step_type"" VARCHAR(50)
);

-- =========================
-- STEP CHARACTER
-- =========================
CREATE TABLE ""StepCharacter"" (
    ""id"" UUID PRIMARY KEY,
    ""character_state_id"" UUID NOT NULL,
    ""transform_id"" UUID
);

-- =========================
-- FOREIGN KEYS
-- =========================

ALTER TABLE ""Novels""
    ADD FOREIGN KEY (""cover_image_id"") REFERENCES ""Images""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Novels""
    ADD FOREIGN KEY (""start_label_id"") REFERENCES ""Labels""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Images""
ADD FOREIGN KEY (""novel_id"") REFERENCES ""Novels""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Characters""
ADD FOREIGN KEY (""novel_id"") REFERENCES ""Novels""(""id"") ON DELETE CASCADE;

ALTER TABLE ""CharacterStates""
ADD FOREIGN KEY (""character_id"") REFERENCES ""Characters""(""id"") ON DELETE CASCADE;

ALTER TABLE ""CharacterStates""
ADD FOREIGN KEY (""image_id"") REFERENCES ""Images""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Labels""
ADD FOREIGN KEY (""novel_id"") REFERENCES ""Novels""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""label_id"") REFERENCES ""Labels""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""menu_id"") REFERENCES ""Menus""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""replica_id"") REFERENCES ""Replicas""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""background_id"") REFERENCES ""Backgrounds""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""character_id"") REFERENCES ""StepCharacter""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Steps""
ADD FOREIGN KEY (""next_label_id"") REFERENCES ""Labels""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Choices""
ADD FOREIGN KEY (""menu_id"") REFERENCES ""Menus""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Choices""
ADD FOREIGN KEY (""next_label_id"") REFERENCES ""Labels""(""id"") ON DELETE CASCADE;

ALTER TABLE ""Replicas""
ADD FOREIGN KEY (""speaker_id"") REFERENCES ""Characters""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Backgrounds""
ADD FOREIGN KEY (""img"") REFERENCES ""Images""(""id"") ON DELETE CASCADE;

ALTER TABLE ""StepCharacter""
ADD FOREIGN KEY (""character_state_id"") REFERENCES ""CharacterStates""(""id"") ON DELETE CASCADE;

ALTER TABLE ""CharacterStates""
    ADD FOREIGN KEY (""transform_id"") REFERENCES ""Transforms""(""id"") ON DELETE SET NULL;

ALTER TABLE ""Backgrounds""
    ADD FOREIGN KEY (""transform_id"") REFERENCES ""Transforms""(""id"") ON DELETE SET NULL;

ALTER TABLE ""StepCharacter""
    ADD FOREIGN KEY (""transform_id"") REFERENCES ""Transforms""(""id"") ON DELETE SET NULL;

-- =========================
-- INDEXES
-- =========================

CREATE INDEX idx_steps_label ON ""Steps""(""label_id"");
CREATE INDEX idx_steps_order ON ""Steps""(""label_id"", ""step_order"");
CREATE INDEX idx_choices_menu ON ""Choices""(""menu_id"");
CREATE INDEX idx_character_states_character ON ""CharacterStates""(""character_id"");
";
    }
}
