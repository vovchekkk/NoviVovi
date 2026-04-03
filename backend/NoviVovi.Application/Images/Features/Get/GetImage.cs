using MediatR;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Features.Upload;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Get;

public record GetImageQuery(
    Guid ImageId
) : IRequest<ImageDto>;

public class GetImageHandler(
    ImageDtoMapper mapper
) : IRequestHandler<GetImageQuery, ImageDto>
{
    public Task<ImageDto> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}