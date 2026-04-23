using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IImageDbORepository
{
    Task<ImageDbO?> GetImageByIdAsync(Guid id);
    Task<BackgroundDbO?> GetFullBackgroundByIdAsync(Guid bgId);
    Task<Guid> AddOrUpdateImageAsync(ImageDbO image);
    Task<Guid> AddOrUpdateTransformAsync(TransformDbO transform);
    Task<Guid> AddOrUpdateBackgroundAsync(BackgroundDbO background);
    Task<TransformDbO?> GetTransformByIdAsync(Guid id);
    Task DeleteImageAsync(Guid imageId);
    Task DeleteTransformById(Guid id);
    Task UpdateImageAsync(ImageDbO testImage);
}