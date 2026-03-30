using MediatR;
using NoviVovi.Application.Steps.Dtos;

namespace NoviVovi.Application.Steps.Features.Get;

public record GetStepsQuery(
    Guid NovelId,
    Guid LabelId
) : IRequest<IEnumerable<StepDto>>;

public class GetStepsHandler : IRequestHandler<GetStepsQuery, IEnumerable<StepDto>>
{
    public Task<IEnumerable<StepDto>> Handle(GetStepsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}