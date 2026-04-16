using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NodeResponseMapper
{
    public partial NodeResponse ToResponse(Node source);
    
    public partial IEnumerable<NodeResponse> ToResponses(IEnumerable<Node> sources);
}