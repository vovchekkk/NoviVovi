using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Domain.Dialogue;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Dialogue.Mappers;

[Mapper]
public partial class ReplicaDtoMapper
{
    public partial ReplicaDto ToDto(Replica subject);

    public partial IEnumerable<ReplicaDto> ToDtos(IEnumerable<Replica> subjects);
}