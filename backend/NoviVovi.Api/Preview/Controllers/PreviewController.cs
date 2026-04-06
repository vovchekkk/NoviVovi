using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Preview.Mappers;
using NoviVovi.Api.Preview.Responses;
using NoviVovi.Application.Preview.Features.Get;

namespace NoviVovi.Api.Preview.Controllers;

[ApiController]
[Tags("Preview")]
[Route("preview/novels/")]
public class PreviewController(
    IMediator mediator,
    SceneStateResponseMapper sceneStateMapper
) : ControllerBase
{
    [HttpGet("{novelId:guid}/labels/{labelId:guid}/steps/{stepId:guid}")]
    public async Task<ActionResult<SceneStateResponse>> GetPreview(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        [FromRoute] Guid stepId)
    {
        var sceneState = await mediator.Send(new GetScenePreviewQuery(novelId, labelId, stepId));
        
        return Ok(sceneStateMapper.ToResponse(sceneState));
    }
}