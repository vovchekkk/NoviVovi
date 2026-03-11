using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterStateMapper
{
    public partial CharacterStateSnapshot ToSnapshot(CharacterState novel);

    public partial IEnumerable<CharacterStateSnapshot> ToSnapshots(IEnumerable<CharacterState> novels);
}