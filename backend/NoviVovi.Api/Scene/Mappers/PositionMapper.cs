using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class PositionResponseMapper
{
    public partial PositionResponse ToResponse(PositionSnapshot subject);
    
    public partial IEnumerable<PositionResponse> ToResponses(IEnumerable<PositionSnapshot> subjects);
}