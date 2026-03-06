using Microsoft.AspNetCore.Mvc;
using NoviVovi.Application.Contracts.Preview;
using NoviVovi.Application.Features.Preview.Choose;
using NoviVovi.Application.Features.Preview.Next;
using NoviVovi.Application.Features.Preview.Start;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/preview")]
public class PreviewController(
    StartPreviewHandler start,
    NextStepHandler next,
    ChooseChoiceHandler choose) : ControllerBase
{
    [HttpPost("start/{novelId}")]
    public async Task<ActionResult<SceneSnapshot>> Start(Guid novelId)
    {
        var result = await start.Handle(new StartPreviewCommand(novelId));
        return Ok(result);
    }

    [HttpPost("{sessionId}/next")]
    public async Task<ActionResult<SceneSnapshot>> Next(Guid sessionId)
    {
        var result = await next.Handle(new NextStepCommand(sessionId));
        return Ok(result);
    }

    [HttpPost("{sessionId}/choice/{choiceId}")]
    public async Task<ActionResult<SceneSnapshot>> Choose(Guid sessionId, Guid choiceId)
    {
        var result = await choose.Handle(
            new ChooseChoiceCommand(sessionId, choiceId)
        );

        return Ok(result);
    }
}