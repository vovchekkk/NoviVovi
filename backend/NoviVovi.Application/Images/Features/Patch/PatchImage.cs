using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;

namespace NoviVovi.Application.Images.Features.Patch;

public class PatchImageCommand : IRequest<ImageDto>
{
    public required Guid ImageId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Format { get; init; }
    public ImageType? Type { get; init; }
    public SizeDto? Size { get; init; }
}

public class PatchImageHandler(
    IImageRepository imageRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ImageDtoMapper mapper
) : IRequestHandler<PatchImageCommand, ImageDto>
{
    public async Task<ImageDto> Handle(PatchImageCommand request, CancellationToken ct)
    {
        var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                    ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
        
        if (request.Name != null)
            image.UpdateName(request.Name);
        
        if (request.Description != null)
            image.UpdateDescription(request.Description);
        
        if (request.Format != null)
            image.UpdateFormat(request.Format);
        
        if (request.Type.HasValue)
            image.UpdateType(request.Type);
        
        if (request.Size != null)
            image.UpdateSize(request.Size.Width, request.Size.Height);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(image);
    }
}