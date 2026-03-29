using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Choose;

public record ChooseChoiceCommand(
    Guid SessionId,
    Guid ChoiceId
) : IRequest<SceneStateDto>;

public class ChooseChoiceHandler(
    PreviewSessionStore sessions,
    ILabelRepository labelRepository,
    SceneStateDtoMapper stateDtoMapper
) : IRequestHandler<ChooseChoiceCommand, SceneStateDto>
{
    public async Task<SceneStateDto> Handle(ChooseChoiceCommand request, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(request.SessionId);
        if (session == null)
        {
            throw new Exception($"Session with id {request.SessionId} not found.");
        }

        session.Player.SelectChoice(request.ChoiceId);

        await session.Player.ExecuteNextAsync(labelRepository);

        return stateDtoMapper.ToDto(session.State);
    }
}