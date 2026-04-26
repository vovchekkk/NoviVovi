using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Domain.Images;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class ImageRepository(IImageDbORepository dboRepo, ImageMapper mapper) : IImageRepository
{
    public async Task<Image?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var dbo = await dboRepo.GetImageByIdAsync(id);
        if (dbo != null) 
            return mapper.ToDomain(dbo);
        return null;
    }

    public async Task AddAsync(Image image, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(image);
        await dboRepo.AddOrUpdateImageAsync(dbo);
    }

    public async Task DeleteAsync(Image image, CancellationToken ct)
    {
        await dboRepo.DeleteImageAsync(image.Id);
    }
}