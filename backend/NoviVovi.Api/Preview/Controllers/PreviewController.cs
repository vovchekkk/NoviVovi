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
    [HttpGet("{novelId:guid}")]
    public async Task<ActionResult<SceneStateResponse>> GetPreview(
        [FromRoute] Guid novelId,
        [FromQuery] Guid labelId,
        [FromQuery] Guid stepId)
    {
        var sceneState = await mediator.Send(new GetScenePreviewQuery(labelId, stepId));
        
        return Ok(sceneStateMapper.ToResponse(sceneState));
    }
}