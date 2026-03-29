using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class SizeResponseMapper
{
    public partial SizeResponse ToResponse(SizeSnapshot subject);
    
    public partial IEnumerable<SizeResponse> ToResponses(IEnumerable<SizeSnapshot> subjects);
}