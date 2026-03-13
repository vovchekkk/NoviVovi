using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Application.Dialogue.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Dialogue.Mappers;

[Mapper]
public partial class ReplicaResponseMapper
{
    public partial ReplicaResponse ToSnapshot(ReplicaSnapshot novel);

    public partial IEnumerable<ReplicaResponse> ToSnapshots(IEnumerable<ReplicaSnapshot> novels);
}