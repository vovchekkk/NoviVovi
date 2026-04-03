using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class ChoiceEdgeResponseMapper
{
    public partial ChoiceEdgeResponse ToResponse(ChoiceEdge subject);
    
    public partial IEnumerable<ChoiceEdgeResponse> ToResponses(IEnumerable<ChoiceEdge> subjects);
}