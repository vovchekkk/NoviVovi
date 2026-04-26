using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Steps.CommandMappers;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Steps.Mappers;
using NoviVovi.Api.Steps.Requests;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Features.Add;
using NoviVovi.Application.Steps.Features.Delete;
using NoviVovi.Application.Steps.Features.Get;

namespace NoviVovi.Api.Steps.Controllers;

[ApiController]
[Tags("Steps")]
[Route("api/novels/{novelId:guid}/labels/{labelId:guid}/steps")]
public class StepsController(
    IMediator mediator,
    StepCommandMapper commandMapper,
    StepResponseMapper mapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        AddStepRequest request
    )
    {
        var command = commandMapper.ToAddCommand(request, novelId, labelId);
        
        var step = await mediator.Send(command);

        return Ok(mapper.ToResponse(step));
    }

    [HttpGet("{stepid:guid}")]
    public async Task<ActionResult<NovelResponse>> Get([FromRoute] Guid novelId, [FromRoute] Guid labelId,
        [FromRoute] Guid stepId)
    {
        var step = await mediator.Send(new GetStepQuery(novelId, labelId, stepId));

        return Ok(mapper.ToResponse(step));
    }

    [HttpGet]
    public async Task<ActionResult<NovelResponse>> Get([FromRoute] Guid novelId, [FromRoute] Guid labelId)
    {
        var step = await mediator.Send(new GetStepsQuery(novelId, labelId));

        return Ok(mapper.ToResponses(step));
    }

    [HttpPatch("{stepid:guid}")]
    public async Task<ActionResult<StepResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        [FromRoute] Guid stepId,
        [FromBody] PatchStepRequest request
    )
    {
        var command = commandMapper.ToCommand((dynamic)request, novelId, labelId, stepId);

        var step = await mediator.Send((dynamic)command);

        return Ok(mapper.ToResponse(step));
    }

    [HttpDelete("{stepid:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        [FromRoute] Guid stepId
    )
    {
        await mediator.Send(new DeleteStepCommand(novelId, labelId, stepId));

        return NoContent();
    }
}