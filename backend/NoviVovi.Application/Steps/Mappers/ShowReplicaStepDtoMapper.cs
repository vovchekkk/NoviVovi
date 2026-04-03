using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowReplicaStepDtoMapper(
    ReplicaDtoMapper replicaMapper
)
{
    public partial ShowReplicaStepDto ToDto(ShowReplicaStep subject);

    private ReplicaDto MapReplica(Replica source) => replicaMapper.ToDto(source);

    public partial IEnumerable<ShowReplicaStepDto> ToDtos(IEnumerable<ShowReplicaStep> subjects);
}