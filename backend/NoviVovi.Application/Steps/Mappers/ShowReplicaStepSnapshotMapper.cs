using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowReplicaStepSnapshotMapper
{
    public partial ShowReplicaStepSnapshot ToSnapshot(ShowReplicaStep subject);
    
    public partial IEnumerable<ShowReplicaStepSnapshot> ToSnapshots(IEnumerable<ShowReplicaStep> subjects);
}