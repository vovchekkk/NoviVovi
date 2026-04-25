using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Labels.Abstractions;

public interface ILabelRepository
{
    public Task<Label?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task<IEnumerable<Label>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
    public Task AddOrUpdateAsync(Label label, CancellationToken ct);
    public Task DeleteAsync(Label label, CancellationToken ct);
}