using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Labels.CommandMappers;
using NoviVovi.Api.Labels.Mappers;
using NoviVovi.Api.Labels.Requests;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Application.Labels.Features.Delete;
using NoviVovi.Application.Labels.Features.Get;

namespace NoviVovi.Api.Labels.Controllers;

[ApiController]
[Tags("Labels")]
[Route("api/novels/{novelId:guid}/labels")]
public class LabelsController(
    IMediator mediator,
    LabelCommandMapper commandMapper,
    LabelResponseMapper mapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LabelResponse>> Add(
        [FromRoute] Guid novelId,
        [FromBody] AddLabelRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId);

        var label = await mediator.Send(command);

        return Ok(mapper.ToResponse(label));
    }

    [HttpGet("{labelId:guid}")]
    public async Task<ActionResult<LabelResponse>> Get(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId
    )
    {
        var label = await mediator.Send(new GetLabelQuery(novelId, labelId));

        return Ok(mapper.ToResponse(label));
    }

    [HttpGet]
    public async Task<ActionResult<LabelResponse>> Get(
        [FromRoute] Guid novelId
    )
    {
        var label = await mediator.Send(new GetLabelsQuery(novelId));

        return Ok(mapper.ToResponses(label));
    }

    [HttpPatch("{labelId:guid}")]
    public async Task<ActionResult<LabelResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        PatchLabelRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId, labelId);

        var label = await mediator.Send(command);

        return Ok(mapper.ToResponse(label));
    }

    [HttpDelete("{labelId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId
    )
    {
        await mediator.Send(new DeleteLabelCommand(novelId, labelId));

        return NoContent();
    }
}