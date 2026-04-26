using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Novels.Responses.Nodes;
using NoviVovi.Application.Novels.Dtos.Nodes;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NodeResponseMapper
{
    public NodeResponse ToResponse(NodeDto source)
    {
        return source switch
        {
            JumpNodeDto jumpNode => ToResponse(jumpNode),
            MenuNodeDto menuNode => ToResponse(menuNode),
            _ => new NodeResponse
            {
                LabelId = source.LabelId,
                LabelName = source.LabelName
            }
        };
    }

    public partial JumpNodeResponse ToResponse(JumpNodeDto source);

    public partial MenuNodeResponse ToResponse(MenuNodeDto source);
}