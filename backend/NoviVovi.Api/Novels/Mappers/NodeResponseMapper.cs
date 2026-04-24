using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Novels.Responses.Nodes;
using NoviVovi.Application.Novels.Dtos.Nodes;
using NoviVovi.Application.Novels.Models;
using NoviVovi.Application.Novels.Models.Nodes;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NodeResponseMapper
{
    public partial NodeResponse ToResponse(Node source);
    
    public partial JumpNodeResponse ToResponse(JumpNodeDto sources);
    
    public partial MenuNodeResponse ToResponse(MenuNodeDto sources);
}