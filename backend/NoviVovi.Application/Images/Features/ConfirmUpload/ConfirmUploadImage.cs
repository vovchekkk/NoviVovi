using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.ConfirmUpload;

public record ConfirmUploadImageCommand(
    Guid ImageId
) : IRequest<ImageDto>;

public class ConfirmUploadImageHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    ImageDtoMapper mapper
) : IRequestHandler<ConfirmUploadImageCommand, ImageDto>
{
    public async Task<ImageDto> Handle(ConfirmUploadImageCommand request, CancellationToken ct)
    {
        var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                    ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
        
        image.ConfirmUpload();
        
        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(image);
    }
}