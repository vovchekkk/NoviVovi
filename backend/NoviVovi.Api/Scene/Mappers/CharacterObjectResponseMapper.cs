using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class CharacterObjectResponseMapper
{
    public partial CharacterObjectSnapshot ToSnapshot(CharacterObject novel);
    
    public partial IEnumerable<CharacterObjectSnapshot> ToSnapshots(IEnumerable<CharacterObject> novels);
}