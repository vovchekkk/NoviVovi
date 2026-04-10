using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IStepDbORepository
{
    Task<IEnumerable<StepDbO>> GetOrderedByLabelIdAsync(Guid labelId);
    Task<IEnumerable<StepDbO>> GetByLabelIdsAsync(IEnumerable<Guid> labelIds);
    Task<IEnumerable<StepDbO>> GetFullByLabelIdsAsync(IEnumerable<Guid> labelIds);
    Task<StepDbO?> GetFullByIdAsync(Guid stepId);

    Task<Guid> AddAsync(StepDbO step);
    Task UpdateAsync(StepDbO step);
    Task DeleteAsync(Guid id);
    Task AddFullAsync(StepDbO step);
}