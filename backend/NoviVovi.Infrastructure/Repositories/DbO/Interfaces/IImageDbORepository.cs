using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IImageDbORepository
{
    Task<ImageDbO?> GetImageByIdAsync(Guid id);
    Task<TransformDbO?> GetTransformByIdAsync(Guid id);
    Task<BackgroundDbO?> GetFullBackgroundByIdAsync(Guid bgId);
    Task<Guid> AddImageAsync(ImageDbO image);
    Task<Guid> AddBgAsync(BackgroundDbO background);
    Task<Guid> AddTransformAsync(TransformDbO stepCharacterTransform);
    Task DeleteImageAsync(Guid imageId);
}