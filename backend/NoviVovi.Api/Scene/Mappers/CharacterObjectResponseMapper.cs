using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class CharacterObjectResponseMapper
{
    public partial CharacterObjectDto ToResponse(CharacterObject subject);
    
    public partial IEnumerable<CharacterObjectDto> ToResponses(IEnumerable<CharacterObject> subjects);
}