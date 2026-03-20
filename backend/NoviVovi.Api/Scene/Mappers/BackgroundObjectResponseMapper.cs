using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectResponseMapper
{
    public partial BackgroundObjectResponse ToResponse(BackgroundObjectSnapshot subject);
    
    public partial IEnumerable<BackgroundObjectResponse> ToResponses(IEnumerable<BackgroundObjectSnapshot> subjects);
}