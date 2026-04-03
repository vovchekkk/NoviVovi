using MediatR;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Upload;

public class UploadImageCommand : IRequest<ImageDto>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Format { get; init; }
    public required string Type { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
}

public class UploadImageHandler(
    ImageDtoMapper mapper
) : IRequestHandler<UploadImageCommand, ImageDto>
{
    public Task<ImageDto> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}