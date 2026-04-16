using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NovelResponseMapper
{
    public partial NovelResponse ToResponse(NovelDto source);
    
    public partial IEnumerable<NovelResponse> ToResponses(IEnumerable<NovelDto> sources);
}