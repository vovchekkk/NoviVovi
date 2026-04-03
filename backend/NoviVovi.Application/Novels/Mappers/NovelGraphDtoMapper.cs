using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelGraphDtoMapper
{
    public partial NovelGraphDto ToDto(NovelGraph subject);
    
    public partial IEnumerable<NovelGraphDto> ToDtos(IEnumerable<NovelGraph> subjects);
}