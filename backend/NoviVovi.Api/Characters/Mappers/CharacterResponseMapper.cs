using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.Mappers;

[Mapper]
public partial class CharacterResponseMapper
{
    public partial CharacterResponse ToSnapshot(CharacterSnapshot novel);
    
    public partial IEnumerable<CharacterResponse> ToSnapshots(IEnumerable<CharacterSnapshot> novels);
}