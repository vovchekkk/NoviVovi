using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.Repositories.DbO.Interfaces;

public interface IStepDbORepository
{
    Task<IEnumerable<StepDbO>> GetOrderedByLabelIdAsync(Guid labelId , LoadContext ctx);
    Task<IEnumerable<StepDbO>> GetFullByLabelIdsAsync(IEnumerable<Guid> labelIds);
    Task<StepDbO?> GetFullByIdAsync(Guid stepId, LoadContext ctx);
    Task DeleteAsync(Guid id);
    Task AddOrUpdateFullAsync(StepDbO step, LoadContext ctx);
    Task DeleteStepAsync(Guid stepId);
    Task<HashSet<Guid>> GetStepIdsByLabelIdAsync(Guid labelId);
    Task UpdateReplicaAsync(ReplicaDbO replica);
    Task<Guid> CreateReplicaAsync(ReplicaDbO replica);
    Task DeleteReplicaAsync(Guid replicaId);
    Task<ReplicaDbO?> GetReplicaByIdAsync(Guid replicaId);
}