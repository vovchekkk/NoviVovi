using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Get;

public record GetImageQuery(
    Guid ImageId
) : IRequest<ImageDto>;

public class GetImageHandler(
    IImageRepository imageRepository,
    IStorageService storageService,
    ImageDtoMapper mapper
) : IRequestHandler<GetImageQuery, ImageDto>
{
    public async Task<ImageDto> Handle(GetImageQuery request, CancellationToken ct)
    {
        var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                    ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
        
        var viewUrl = storageService.GetViewUrl(image.StoragePath);
        
        return mapper.ToDto(image);
    }
}