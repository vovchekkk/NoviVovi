using NoviVovi.Application.Labels;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Infrastructure.Labels;

public class LabelRepository : ILabelRepository
{
    public Task<Label?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Label label, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Label label, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByNovelIdAsync(Guid novelId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}