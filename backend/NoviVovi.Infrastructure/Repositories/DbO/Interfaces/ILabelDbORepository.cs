using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface ILabelDbORepository
{
    Task<IEnumerable<LabelDbO?>> GetFullByNovelIdAsync(Guid novelId); // с полными Step'ами
    Task<IEnumerable<LabelDbO>> GetFullByNovelIdsAsync(IEnumerable<Guid> novelIds);
    Task<IEnumerable<LabelDbO>> GetFullByIdsAsync(IEnumerable<Guid> ids);

    Task<LabelDbO?> GetFullByIdAsync(Guid id, LoadContext ctx);
    
    // Task<Guid> AddAsync(LabelDbO label);
    // Task UpdateAsync(LabelDbO label);
    Task DeleteAsync(Guid id);
    Task<Guid> AddOrUpdateFullAsync(LabelDbO label, LoadContext? ctx = null);
}