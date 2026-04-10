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

    public async Task AddAsync(Label label, Guid novelId, CancellationToken ct)
    {
        var dbo = mapper.ToDbO(label, novelId);
        await dboRepo.AddFullAsync(dbo);
    }

    public async Task DeleteAsync(Label label, CancellationToken ct)
    {
        await dboRepo.DeleteAsync(label.Id);
    }

    public Task DeleteByNovelIdAsync(Guid novelId, CancellationToken ct)
    {
        throw new NotImplementedException(); //а это зачем? У нас при удалении новеллы и так все удалится
    }
}