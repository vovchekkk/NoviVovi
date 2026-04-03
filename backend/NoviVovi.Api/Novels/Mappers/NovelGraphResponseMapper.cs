using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NovelGraphResponseMapper
{
    public partial NovelGraphResponse ToResponse(NovelGraphDto subject);
    
    public partial IEnumerable<NovelGraphResponse> ToResponses(IEnumerable<NovelGraphDto> subjects);
}