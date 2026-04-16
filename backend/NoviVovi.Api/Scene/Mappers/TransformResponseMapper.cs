using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class TransformResponseMapper
{
    public partial TransformResponse ToResponse(TransformDto source);
    
    public partial IEnumerable<TransformResponse> ToResponses(IEnumerable<TransformDto> sources);
}