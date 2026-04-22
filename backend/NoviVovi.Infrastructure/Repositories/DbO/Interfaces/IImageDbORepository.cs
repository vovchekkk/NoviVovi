using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IImageDbORepository
{
    Task<ImageDbO?> GetImageByIdAsync(Guid id);
    Task<BackgroundDbO?> GetFullBackgroundByIdAsync(Guid bgId);
    Task<Guid> AddImageAsync(ImageDbO image);
    Task<Guid> AddOrUpdateImageAsync(ImageDbO image);
    Task<Guid> AddBgAsync(BackgroundDbO background);
    Task<Guid> AddTransformAsync(TransformDbO transform);
    Task<Guid> AddOrUpdateTransformAsync(TransformDbO transform);
    Task<TransformDbO?> GetTransformByIdAsync(Guid id);
    Task DeleteImageAsync(Guid imageId);
    Task DeleteTransformById(Guid id);
    Task UpdateImageAsync(ImageDbO testImage);
}