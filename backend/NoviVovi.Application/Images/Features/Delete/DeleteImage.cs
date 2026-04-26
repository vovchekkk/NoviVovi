using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Delete;

public record DeleteImageCommand(
    Guid ImageId
) : IRequest;

public class DeleteImageHandler(
    IImageRepository imageRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteImageCommand>
{
    public async Task Handle(DeleteImageCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                        ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
            
            await imageRepository.DeleteAsync(image, ct);
            await unitOfWork.CommitAsync(ct);
            
            await storageService.DeleteFileAsync(image.StoragePath, ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}