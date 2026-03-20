using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NovelResponseMapper
{
    public partial NovelResponse ToResponse(NovelSnapshot subject);
    
    public partial IEnumerable<NovelResponse> ToResponses(IEnumerable<NovelSnapshot> subjects);
}