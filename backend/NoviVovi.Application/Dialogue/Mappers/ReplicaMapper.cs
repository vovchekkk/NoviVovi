using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Domain.Dialogue;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Dialogue.Mappers;

[Mapper]
public partial class ReplicaMapper
{
    public partial ReplicaSnapshot ToSnapshot(Replica novel);

    public partial IEnumerable<ReplicaSnapshot> ToSnapshots(IEnumerable<Replica> novels);
}