using NoviVovi.Domain.Images;

namespace NoviVovi.Application.Images.Abstractions;

public interface IImageRepository
{
    public Task<Image?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddAsync(Image image, CancellationToken ct);
    public Task DeleteAsync(Image image, CancellationToken ct);
}