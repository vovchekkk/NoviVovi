using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class ChoiceEdgeDtoMapper
{
    public partial ChoiceEdgeDto ToDto(ChoiceEdge subject);
    
    public partial IEnumerable<ChoiceEdgeDto> ToDtos(IEnumerable<ChoiceEdge> subjects);
}