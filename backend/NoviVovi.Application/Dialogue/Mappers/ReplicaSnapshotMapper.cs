using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Domain.Dialogue;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Dialogue.Mappers;

[Mapper]
public partial class ReplicaSnapshotMapper
{
    public partial ReplicaSnapshot ToSnapshot(Replica subject);

    public partial IEnumerable<ReplicaSnapshot> ToSnapshots(IEnumerable<Replica> subjects);
}