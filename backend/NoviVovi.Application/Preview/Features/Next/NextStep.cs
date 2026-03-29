using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Next;

public record NextStepCommand(
    Guid SessionId
) : IRequest<SceneStateDto>;

public class NextStepHandler(
    PreviewSessionStore sessions,
    ILabelRepository labelRepository,
    SceneStateDtoMapper stateDtoMapper
) : IRequestHandler<NextStepCommand, SceneStateDto>
{
    public async Task<SceneStateDto> Handle(NextStepCommand request, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(request.SessionId);
        if (session == null)
        {
            throw new Exception($"Session with id {request.SessionId} not found.");
        }

        await session.Player.ExecuteNextAsync(labelRepository);

        await sessions.SaveAsync(session);

        return stateDtoMapper.ToDto(session.State);
    }
}