using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class CharacterObjectResponseMapper
{
    public partial CharacterObjectSnapshot ToResponse(CharacterObject subject);
    
    public partial IEnumerable<CharacterObjectSnapshot> ToResponses(IEnumerable<CharacterObject> subjects);
}