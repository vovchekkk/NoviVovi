using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Domain.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelDtoMapper
{
    public partial NovelDto ToDto(Novel subject);
    
    public partial IEnumerable<NovelDto> ToDtos(IEnumerable<Novel> subjects);
}