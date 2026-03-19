using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class TransformResponseMapper
{
    public partial TransformResponse ToResponse(TransformSnapshot novel);
    
    public partial IEnumerable<TransformResponse> ToResponses(IEnumerable<TransformSnapshot> novels);
}