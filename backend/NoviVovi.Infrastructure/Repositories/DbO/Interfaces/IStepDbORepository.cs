using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IStepDbORepository
{
    Task<IEnumerable<StepDbO>> GetOrderedByLabelIdAsync(Guid labelId , LoadContext ctx);
    Task<IEnumerable<StepDbO>> GetByLabelIdsAsync(IEnumerable<Guid> labelIds);
    Task<IEnumerable<StepDbO>> GetFullByLabelIdsAsync(IEnumerable<Guid> labelIds);
    Task<StepDbO?> GetFullByIdAsync(Guid stepId, LoadContext ctx);
    Task DeleteAsync(Guid id);
    Task AddOrUpdateFullAsync(StepDbO step, LoadContext ctx);
}