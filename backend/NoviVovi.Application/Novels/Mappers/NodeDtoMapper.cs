using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Dtos.Nodes;
using NoviVovi.Application.Novels.Models.Nodes;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NodeDtoMapper
{
    [MapDerivedType(typeof(MenuNode), typeof(MenuNodeDto))]
    [MapDerivedType(typeof(JumpNode), typeof(JumpNodeDto))]
    public partial NodeDto ToDto(Node source);
    
    public partial MenuNodeDto ToDto(MenuNode source);
    
    public partial JumpNodeDto ToDto(JumpNode source);
}