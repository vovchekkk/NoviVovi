using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.Mappers;

[Mapper]
public partial class UploadInfoImageResponseMapper
{
    public partial UploadInfoImageResponse ToResponse(UploadInfoImageDto source);

    public partial IEnumerable<UploadInfoImageResponse> ToResponses(IEnumerable<UploadInfoImageDto> sources);
}