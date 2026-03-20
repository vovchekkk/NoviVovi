using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class CharacterObjectMapper
{
    public partial CharacterObjectSnapshot ToSnapshot(CharacterObject subject);
    
    public partial IEnumerable<CharacterObjectSnapshot> ToSnapshots(IEnumerable<CharacterObject> subjects);
}