using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Infrastructure;
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
    [Produces("application/json")]
    public async Task<ActionResult<StepResponse>> Create(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        AddStepRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId, labelId);
        
        var step = await mediator.Send(command);

        StepResponse response = mapper.ToResponse(step);
        
        return response;
    }

    [HttpGet("{stepid:guid}")]
    [Produces("application/json")]
    public async Task<ActionResult<StepResponse>> Get([FromRoute] Guid novelId, [FromRoute] Guid labelId,
        [FromRoute] Guid stepId)
    {
        var step = await mediator.Send(new GetStepQuery(novelId, labelId, stepId));

        StepResponse response = mapper.ToResponse(step);
        return response;
    }

    [HttpGet]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<StepResponse>>> Get([FromRoute] Guid novelId, [FromRoute] Guid labelId)
    {
        var step = await mediator.Send(new GetStepsQuery(novelId, labelId));

        var responses = mapper.ToResponses(step);
        return Ok(responses);
    }

    [HttpPatch("{stepid:guid}")]
    [Produces("application/json")]
    public async Task<ActionResult<StepResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid labelId,
        [FromRoute] Guid stepId,
        [FromBody] PatchStepRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId, labelId, stepId);

        var step = await mediator.Send(command);

        StepResponse response = mapper.ToResponse(step);
        return response;
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