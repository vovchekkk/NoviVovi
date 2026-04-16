using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Dialogue.Mappers;

[Mapper]
public partial class ReplicaDtoMapper(
    CharacterDtoMapper characterMapper
)
{
    public partial ReplicaDto ToDto(Replica source);

    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);

    public partial IEnumerable<ReplicaDto> ToDtos(IEnumerable<Replica> sources);
}