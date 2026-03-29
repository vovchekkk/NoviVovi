using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.Mappers;

[Mapper]
public partial class CharacterResponseMapper
{
    public partial CharacterResponse ToResponse(CharacterSnapshot subject);
    
    public partial IEnumerable<CharacterResponse> ToResponses(IEnumerable<CharacterSnapshot> subjects);
}