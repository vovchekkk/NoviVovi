using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NodeDtoMapper
{
    public partial NodeDto ToDto(Node subject);
    
    public partial IEnumerable<NodeDto> ToDtos(IEnumerable<Node> subjects);
}