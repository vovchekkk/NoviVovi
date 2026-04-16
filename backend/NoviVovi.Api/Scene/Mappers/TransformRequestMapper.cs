using NoviVovi.Api.Scene.Requests;
using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class TransformRequestMapper
{
    public partial TransformDto ToDto(TransformRequest source);
    
    public partial IEnumerable<TransformDto> ToDto(IEnumerable<TransformRequest> sources);
}