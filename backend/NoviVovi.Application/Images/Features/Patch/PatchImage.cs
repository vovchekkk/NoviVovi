using MediatR;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Patch;

public class PatchImageCommand : IRequest<ImageDto>
{
    public required Guid ImageId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Format { get; init; }
    public string? Type { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
}

public class PatchImageHandler(
    ImageDtoMapper mapper
) : IRequestHandler<PatchImageCommand, ImageDto>
{
    public Task<ImageDto> Handle(PatchImageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}