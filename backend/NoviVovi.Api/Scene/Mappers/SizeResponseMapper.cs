using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class SizeResponseMapper
{
    public partial SizeResponse ToResponse(SizeDto source);
    
    public partial IEnumerable<SizeResponse> ToResponses(IEnumerable<SizeDto> sources);
}