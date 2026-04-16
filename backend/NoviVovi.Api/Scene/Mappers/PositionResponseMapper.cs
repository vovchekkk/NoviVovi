using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class PositionResponseMapper
{
    public partial PositionResponse ToResponse(PositionDto source);
    
    public partial IEnumerable<PositionResponse> ToResponses(IEnumerable<PositionDto> sources);
}