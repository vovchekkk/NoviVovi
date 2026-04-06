using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class EdgeDtoMapper
{
    [MapDerivedType(typeof(JumpEdge), typeof(JumpEdgeDto))]
    [MapDerivedType(typeof(ChoiceEdge), typeof(ChoiceEdgeDto))]
    public partial EdgeDto ToDto(Edge source);
}