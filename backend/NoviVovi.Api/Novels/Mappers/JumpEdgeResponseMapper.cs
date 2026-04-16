using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class JumpEdgeResponseMapper
{
    public partial JumpEdgeResponse ToResponse(JumpEdge source);
    
    public partial IEnumerable<JumpEdgeResponse> ToResponses(IEnumerable<JumpEdge> sources);
}