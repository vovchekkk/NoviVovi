using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterMapper
{
    public partial CharacterSnapshot ToSnapshot(Character novel);
    
    public partial IEnumerable<CharacterSnapshot> ToSnapshots(IEnumerable<Character> novels);
}