using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Domain.Images;

namespace NoviVovi.Infrastructure.Repositories;

public class ImageRepository : IImageRepository
{
    public Task<Image?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Image image, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Image image, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}