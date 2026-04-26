using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
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
    public ImageType? Type { get; init; }
}

public class PatchImageHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    ImageDtoMapper mapper
) : IRequestHandler<PatchImageCommand, ImageDto>
{
    public async Task<ImageDto> Handle(PatchImageCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                        ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
            
            if (request.Name is not null)
                image.UpdateName(request.Name);
            
            if (request.Description is not null)
                image.UpdateDescription(request.Description);
            
            if (request.Type.HasValue)
                image.UpdateType(request.Type.Value);
            
            await imageRepository.AddOrUpdateAsync(image, ct);
            
            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(image);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}