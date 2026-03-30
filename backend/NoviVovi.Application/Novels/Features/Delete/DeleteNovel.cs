using MediatR;

namespace NoviVovi.Application.Novels.Features.Delete;

public record DeleteNovelCommand(
    Guid NovelId
) : IRequest;

public class DeleteNovelHandler : IRequestHandler<DeleteNovelCommand>
{
    public async Task Handle(DeleteNovelCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}