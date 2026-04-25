using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Domain.Labels;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

namespace NoviVovi.Infrastructure.Repositories;

public class LabelRepository(ILabelDbORepository dboRepo, LabelMapper mapper) : ILabelRepository
{
    public async Task<Label?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var ctx = new LoadContext();
        var dbo = await dboRepo.GetFullByIdAsync(id, ctx);
        return  dbo == null ? null : mapper.ToDomain(dbo, new MappingContext());
    }

    public async Task<IEnumerable<Label>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var dbos = await dboRepo.GetFullByIdsAsync(ids);
        return dbos.Select(dbo => mapper.ToDomain(dbo, new MappingContext()));
    }

    public async Task AddOrUpdateAsync(Label label, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(label, new MappingContext());
        await dboRepo.AddOrUpdateFullAsync(dbo);
    }

    public async Task DeleteAsync(Label label, CancellationToken ct)
    {
        await dboRepo.DeleteAsync(label.Id);
    }
}