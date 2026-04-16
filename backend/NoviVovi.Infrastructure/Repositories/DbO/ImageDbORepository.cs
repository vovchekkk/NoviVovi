using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories.DbO;

public class ImageDbORepository(
    DatabaseOptions options
) : BaseRepository(options), IImageDbORepository
{
    public async Task<ImageDbO?> GetImageByIdAsync(Guid id)
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
        return await QueryFirstOrDefaultAsync<ImageDbO>(imageSql, new { ImageId = id });
    }

    public async Task<TransformDbO?> GetTransformByIdAsync(Guid id)
    {
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

        return await QueryFirstOrDefaultAsync<TransformDbO>(sql, new { Id = id });
    }


    public async Task DeleteImageAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Images\" WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateImageAsync(ImageDbO image)
    {
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

        await ExecuteAsync(sql, image);
    }

    public async Task<Guid> AddImageAsync(ImageDbO image)
    {
        const string sql = @"
            INSERT INTO ""Images"" (id, novel_id, name, ""URL"", format, img_type, height, width, size)
            VALUES (@Id, @NovelId, @Name, @Url, @Format, @ImgType, @Height, @Width, @Size)";

        await ExecuteAsync(sql, image);
        return image.Id;
    }

    public async Task<Guid> AddBgAsync(BackgroundDbO background)
    {
        const string sql = @"
            INSERT INTO ""Backgrounds"" (id, img, transform_id)
            VALUES (@Id, @Img, @TransformId)";

        if (background.Image != null) await AddImageAsync(background.Image);
        if (background.Transform != null) await AddTransformAsync(background.Transform);
        await ExecuteAsync(sql, background);
        return background.Id;
    }

    public async Task<Guid> AddTransformAsync(TransformDbO transform)
    {
        const string sql = @"
            INSERT INTO ""Transforms"" (id, scale, rotation, z_index, width, height, x_pos, y_pos)
            VALUES (@Id, @Scale, @Rotation, @ZIndex, @Width, @Height, @XPos, @YPos)";
        await ExecuteAsync(sql, transform);
        return transform.Id;
    }
}