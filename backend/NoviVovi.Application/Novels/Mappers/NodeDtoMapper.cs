using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Dtos.Nodes;
using NoviVovi.Application.Novels.Models.Nodes;
using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NodeDtoMapper
{
    public NodeDto ToDto(Node source)
    {
        return source switch
        {
            MenuNode menuNode => ToDto(menuNode),
            JumpNode jumpNode => ToDto(jumpNode),
            _ => new NodeDto(source.LabelId, source.LabelName)
        };
    }
    
    public partial MenuNodeDto ToDto(MenuNode source);
    
    public partial JumpNodeDto ToDto(JumpNode source);
}