using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Labels.CommandMappers;
using NoviVovi.Api.Labels.Mappers;
using NoviVovi.Api.Labels.Requests.Add;
using NoviVovi.Api.Labels.Requests.Patch;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Features.Delete;
using NoviVovi.Application.Labels.Features.Get;
using NoviVovi.Application.Labels.Features.Patch;

namespace NoviVovi.Api.Labels.Controllers;

[ApiController]
[Tags("Labels")]
[Route("api/novels/{novelId:guid}/labels")]
public class LabelsController(
    IMediator mediator,
    AddCommandMapper addCommandMapper,
    PatchCommandMapper patchCommandMapper,
    LabelResponseMapper novelMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LabelResponse>> Add(
        [FromRoute] Guid novelId,
        [FromBody] AddLabelRequest request
    )
    {
        var command = addCommandMapper.ToCommand(request, novelId);

        var label = await mediator.Send(command);

        return Ok(novelMapper.ToResponse(label));
    }

    [HttpGet("{labelId:guid}")]
    public async Task<ActionResult<LabelResponse>> Get(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId
    )
    {
        var label = await mediator.Send(new GetLabelQuery(novelId, labelId));

        return Ok(novelMapper.ToResponse(label));
    }

    [HttpGet]
    public async Task<ActionResult<LabelResponse>> Get(
        [FromRoute] Guid novelId
    )
    {
        var label = await mediator.Send(new GetLabelsQuery(novelId));

        return Ok(novelMapper.ToResponses(label));
    }

    [HttpPatch("{labelId:guid}")]
    public async Task<ActionResult<LabelResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        PatchLabelRequest request
    )
    {
        var command = patchCommandMapper.ToCommand(request, novelId, labelId);

        var label = await mediator.Send(command);

        return Ok(novelMapper.ToResponse(label));
    }

    [HttpDelete("{labelId:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId
    )
    {
        await mediator.Send(new DeleteLabelCommand(novelId, labelId));

        return Ok();
    }
}