using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Dtos.Edges;
using NoviVovi.Application.Novels.Models;
using NoviVovi.Application.Novels.Models.Edges;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class EdgeDtoMapper
{
    [MapDerivedType(typeof(JumpEdge), typeof(JumpEdgeDto))]
    [MapDerivedType(typeof(ChoiceEdge), typeof(ChoiceEdgeDto))]
    public partial EdgeDto ToDto(Edge source);
    
    public partial ChoiceEdgeDto ToDto(ChoiceEdge source);
    
    public partial JumpEdgeDto ToDto(JumpEdge source);
}