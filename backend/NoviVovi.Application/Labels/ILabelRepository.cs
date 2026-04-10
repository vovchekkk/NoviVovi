using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Labels;

public interface ILabelRepository
{
    public Task<Label?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task AddAsync(Label label, Guid novelId, CancellationToken ct);
    public Task DeleteAsync(Label label, CancellationToken ct);
    Task DeleteByNovelIdAsync(Guid novelId, CancellationToken ct); 
}