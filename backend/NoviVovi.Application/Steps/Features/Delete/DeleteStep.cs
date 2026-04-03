using MediatR;
using NoviVovi.Application.Steps.Dtos;

namespace NoviVovi.Application.Steps.Features.Delete;

public record DeleteStepCommand(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
) : IRequest;

public class DeleteStepHandler : IRequestHandler<DeleteStepCommand>
{
    public Task Handle(DeleteStepCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}