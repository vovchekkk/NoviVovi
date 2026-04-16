using NoviVovi.Application.Labels;
using NoviVovi.Domain.Labels;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class LabelRepository(ILabelDbORepository dboRepo, LabelMapper mapper) : ILabelRepository
{
    public async Task<Label?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var dbo = await dboRepo.GetFullByIdAsync(id);
        return  dbo == null ? null : mapper.ToDomain(dbo);
    }

    public Task<IEnumerable<Label>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Label label, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Label label, CancellationToken ct)
    {
        await dboRepo.DeleteAsync(label.Id);
    }
}