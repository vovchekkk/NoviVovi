using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Preview.Mappers;
using NoviVovi.Api.Preview.Responses;
using NoviVovi.Application.Preview.Features.Choose;
using NoviVovi.Application.Preview.Features.Next;
using NoviVovi.Application.Preview.Features.Start;

namespace NoviVovi.Api.Preview.Controllers;

[ApiController]
[Route("api/preview")]
public class PreviewController(
    IMediator mediator,
    SceneStateResponseMapper sceneStateMapper
) : ControllerBase
{
    [HttpPost("start/{novelId:guid}")]
    public async Task<ActionResult<SceneStateResponse>> Start([FromRoute] Guid novelId)
    {
        var result = await mediator.Send(new StartPreviewCommand(novelId));
        return Ok(sceneStateMapper.ToResponse(result));
    }

    [HttpPost("{sessionId:guid}/next")]
    public async Task<ActionResult<SceneStateResponse>> Next([FromRoute] Guid sessionId)
    {
        var result = await mediator.Send(new NextStepCommand(sessionId));
        return Ok(sceneStateMapper.ToResponse(result));
    }

    [HttpPost("{sessionId:guid}/choice/{choiceId:guid}")]
    public async Task<ActionResult<SceneStateResponse>> Choose([FromRoute] Guid sessionId, [FromRoute] Guid choiceId)
    {
        var result = await mediator.Send(new ChooseChoiceCommand(sessionId, choiceId));
        return Ok(sceneStateMapper.ToResponse(result));
    }
}