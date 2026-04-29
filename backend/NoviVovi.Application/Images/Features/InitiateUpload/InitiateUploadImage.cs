using System.Drawing;
using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Images.Models;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;
using Size = NoviVovi.Domain.Scene.Size;

namespace NoviVovi.Application.Images.Features.InitiateUpload;

public class InitiateUploadImageCommand : IRequest<UploadInfoImageDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public required ImageType Type { get; init; }
    public required SizeDto Size { get; init; }
}

public class InitiateUploadImageHandler(
    IImageRepository imageRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    UploadInfoImageDtoMapper mapper
) : IRequestHandler<InitiateUploadImageCommand, UploadInfoImageDto>
{
    public async Task<UploadInfoImageDto> Handle(InitiateUploadImageCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var imageId = Guid.NewGuid();
            var storagePath = $"novels/images/{imageId}.{request.Format}";

            // 2. Создаем запись в БД (Status = Pending)
            var image = Image.CreatePending(
                request.Name,
                request.NovelId,
                storagePath,
                request.Format,
                request.Type,
                new Size(request.Size.Width, request.Size.Height),
                imageId
            );

            await imageRepository.AddOrUpdateAsync(image, ct);
            
            await unitOfWork.CommitAsync(ct);
            
            // 3. Генерируем presigned URL для загрузки
            var uploadUrl = await storageService.GetPresignedUploadUrlAsync(storagePath, ct);
            var viewUrl = storageService.GetViewUrl(storagePath);

            var uploadInfo = new UploadInfoImage
            {
                ImageId = imageId,
                UploadUrl = uploadUrl,
                ViewUrl = viewUrl,
            };

            return mapper.ToDto(uploadInfo);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}