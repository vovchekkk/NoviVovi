using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class JumpEdgeDtoMapper
{
    public partial JumpEdgeDto ToDto(JumpEdge source);
    
    public partial IEnumerable<JumpEdgeDto> ToDtos(IEnumerable<JumpEdge> sources);
}