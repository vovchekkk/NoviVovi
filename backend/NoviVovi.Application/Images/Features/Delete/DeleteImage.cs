using MediatR;
using NoviVovi.Application.Images.Mappers;

namespace NoviVovi.Application.Images.Features.Delete;

public record DeleteImageCommand(
    Guid ImageId
) : IRequest;

public class DeleteImageHandler(
    ImageDtoMapper mapper
) : IRequestHandler<DeleteImageCommand>
{
    public Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}