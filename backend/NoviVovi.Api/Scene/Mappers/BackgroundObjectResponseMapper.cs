using NoviVovi.Api.Scene.Responses;
using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectResponseMapper
{
    public partial BackgroundObjectResponse ToResponse(BackgroundObjectDto source);
    
    public partial IEnumerable<BackgroundObjectResponse> ToResponses(IEnumerable<BackgroundObjectDto> sources);
}