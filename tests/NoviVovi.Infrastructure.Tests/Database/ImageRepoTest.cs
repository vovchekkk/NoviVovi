using Dapper;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Tests.Tests;
using Npgsql;
using XUnitPriorityOrderer;

namespace NoviVovi.Infrastructure.Tests.Database;

public class ImageRepoTest :  IAsyncLifetime
{
    private readonly IServiceProvider provider;
    private readonly string connectionString;
    private HashSet<Guid> imageIds = new();
    private HashSet<Guid> novelIds = new();
    private HashSet<Guid> transformsIds = new();

    public ImageRepoTest()
    {
        provider = TestHelper.CreateProvider();
        var options = provider.GetRequiredService<DatabaseOptions>();
        connectionString = options.ConnectionString;
    }

    private async Task<Guid> CreateNovel()
    {
        var novelId = Guid.NewGuid();

        await using var conn = new NpgsqlConnection(connectionString);
        
        const string insertNovelSql = @"Insert into ""Novels"" (id, is_Public, title) values (@Id, @IsPublic, @Title)";
        
        var novel = new NovelDbO
        {
            Id = novelId,
            IsPublic = true,
            Title = "abobiki"
        };
        await conn.ExecuteAsync(insertNovelSql, novel);
        novelIds.Add(novelId);
        return novelId;
    }

    private async Task<Guid> CreateImage(Guid novelId)
    {
        var imageId = Guid.NewGuid();

        const string insertSql = @"
            INSERT INTO ""Images"" 
            (id, novel_id, name, url, format, img_type, height, width, size)
            VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)";

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
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(insertSql, testImage);
        imageIds.Add(imageId);
        return imageId;
    }
    
    [Fact]
    [Order(1)]
    public async Task TestGetImage()
    {
        var novelId = await CreateNovel();
        var imageId = await CreateImage(novelId);
        var repo = provider.GetRequiredService<IImageDbORepository>();
        
        var result = await repo.GetImageByIdAsync(imageId);
        
        Assert.NotNull(result);
        Assert.Equal(imageId, result.Id);
        Assert.Equal("test", result.Name);
        Assert.Equal("png", result.Format);
        Assert.Equal("test/img", result.Url);
        Assert.Equal("background", result.ImgType);
        Assert.Equal(100, result.Height);
        Assert.Equal(100, result.Width);
        Assert.Equal(1, result.Size);
    }

    [Fact]
    [Order(2)]
    public async Task TestDeleteImage()
    {
        var novelId = await CreateNovel();
        var imageId = await CreateImage(novelId);
        var repo = provider.GetRequiredService<IImageDbORepository>();
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string countScript = @"SELECT COUNT(*) FROM ""Images"" WHERE id = @Id";
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = imageId });
        Assert.True(before > 0);
        await repo.DeleteImageAsync(imageId);
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = imageId });

        Assert.Equal(0, after);
    }

    [Fact]
    [Order(3)]
    public async Task TestCreateImage() //если получение из дб не работает, этот тест тоже сломается
    {
        var novelId = await CreateNovel();
        var repo = provider.GetRequiredService<IImageDbORepository>();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string countScript = @"SELECT COUNT(*) FROM ""Images"" WHERE id = @Id";
        var id = Guid.NewGuid();
        imageIds.Add(id);
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.Equal(0, before);
        var testImage = new ImageDbO
        {
            Id = id,
            NovelId = novelId,
            Name = "test",
            Url = "test/img",
            Format = "png",
            ImgType = "background",
            Height = 100,
            Width = 100,
            Size = 1
        };
        await repo.AddImageAsync(testImage);
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.True(after > 0);
        
        var result = await repo.GetImageByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(testImage.Id, result.Id);
        Assert.Equal(testImage.Name, result.Name);
        Assert.Equal(testImage.Format, result.Format);
        Assert.Equal(testImage.Url, result.Url);
        Assert.Equal(testImage.ImgType, result.ImgType);
        Assert.Equal(testImage.Height, result.Height);
        Assert.Equal(testImage.Width, result.Width);
        Assert.Equal(testImage.Size, result.Size);
    }
    
    
    [Fact]
    public async Task TestUpdateImageAsync()
    {
        var novelId = await CreateNovel();
        var imageId = await CreateImage(novelId);
        
        var testImage = new ImageDbO
        {
            Id = imageId,
            NovelId = novelId,
            Name = "changed",
            Url = "test/img",
            Format = "png",
            ImgType = "background",
            Height = 100,
            Width = 100,
            Size = 1
        };
        
        var repo = provider.GetRequiredService<IImageDbORepository>();
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string countScript = @"SELECT COUNT(*) FROM ""Images"" WHERE id = @Id";
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = imageId });
        Assert.True(before > 0);
        await repo.UpdateImageAsync(testImage);
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = imageId });
        Assert.True(after==before);
        var result = await repo.GetImageByIdAsync(imageId);
        Assert.NotNull(result);
        Assert.Equal(testImage.Id, result.Id);
        Assert.Equal(testImage.Name, result.Name);
    }

    private async Task<Guid> CreateTransform()
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
        var sql =
            @"INSERT INTO ""Transforms"" (id, height, width, scale, x_pos, y_pos) 
                Values (@Id, @Height, @Width, @Scale, @XPos, @YPos)";
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(sql, transform);
        transformsIds.Add(id);
        return id;
    }

    [Fact]
    public async Task TestGetTransformById()
    {
        var id = await CreateTransform();
        var repo = provider.GetRequiredService<IImageDbORepository>();
        var transform = new TransformDbO
        {
            Id = id,
            Height = 100,
            Width = 100,
            Scale = 1,
            XPos = 0,
            YPos = 0
        };
        
        var result = await repo.GetTransformByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(transform.Id, result.Id);
        Assert.Equal(transform.Height, result.Height);
        Assert.Equal(transform.Width, result.Width);
        Assert.Equal(transform.Scale, result.Scale);
        Assert.Equal(transform.XPos, result.XPos);
        Assert.Equal(transform.YPos, result.YPos);
    }

    [Fact]
    public async Task TestTransformCreation()
    {
        var id = Guid.NewGuid();
        transformsIds.Add(id);
        var transform = new TransformDbO
        {
            Id = id,
            Height = 100,
            Width = 100,
            Scale = 1,
            XPos = 0,
            YPos = 0
        };
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        const string countScript = @"SELECT COUNT(*) FROM ""Transforms"" WHERE id = @Id";
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.Equal(0, before);
        
        var repo = provider.GetRequiredService<IImageDbORepository>();
        await repo.AddTransformAsync(transform);
        
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.True(after > 0);
        
        var result = await repo.GetTransformByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(transform.Id, result.Id);
        Assert.Equal(transform.Height, result.Height);
        Assert.Equal(transform.Width, result.Width);
        Assert.Equal(transform.Scale, result.Scale);
        Assert.Equal(transform.XPos, result.XPos);
        Assert.Equal(transform.YPos, result.YPos);
    }

    [Fact]
    public async Task TestTransformDelete()
    {
        var id = await CreateTransform();
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        const string countScript = @"SELECT COUNT(*) FROM ""Transforms"" WHERE id = @Id";
        
        var before = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.True(before > 0);
        
        var repo = provider.GetRequiredService<IImageDbORepository>();
        await repo.DeleteTransformById(id);
        
        var after = await conn.QueryFirstAsync<int>(countScript, new { Id = id });
        Assert.Equal(0, after);
        
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        try
        {
            var imgSql = "DELETE FROM \"Images\" WHERE id = @Id";
            var transSql = "DELETE FROM \"Transforms\" WHERE id = @Id";
            var novelSql = "DELETE FROM \"Novels\" WHERE id = @Id";
            await using var conn = new NpgsqlConnection(connectionString);
            foreach (var trans in transformsIds)
            {
                await conn.ExecuteAsync(transSql, new { Id = trans });
            }

            foreach (var img in imageIds)
            {
                await conn.ExecuteAsync(imgSql, new { Id = img });
            }

            foreach (var novel in novelIds)
            {
                await conn.ExecuteAsync(novelSql, new { Id = novel });
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
}