using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Novels.CommandMappers;
using NoviVovi.Api.Novels.Mappers;
using NoviVovi.Api.Novels.Requests;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Features.Create;
using NoviVovi.Application.Novels.Features.Delete;
using NoviVovi.Application.Novels.Features.Get;
using NoviVovi.Application.Novels.Features.Patch;

namespace NoviVovi.Api.Novels.Controllers;

[ApiController]
[Tags("Novels")]
[Route("api/novels")]
public class NovelsController(
    IMediator mediator,
    CreateNovelCommandMapper createNovelCommandMapper,
    PatchNovelCommandMapper patchNovelCommandMapper,
    NovelResponseMapper mapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create(
        [FromBody] CreateNovelRequest request
    )
    {
        var command = createNovelCommandMapper.ToCommand(request);
        
        var novel = await mediator.Send(command);

        return Ok(mapper.ToResponse(novel));
    }

    [HttpGet("{novelId:guid}")]
    public async Task<ActionResult<NovelResponse>> Get(
        [FromRoute] Guid novelId
    )
    {
        var novel = await mediator.Send(new GetNovelQuery(novelId));

        return Ok(mapper.ToResponse(novel));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NovelResponse>>> Get()
    {
        var novel = await mediator.Send(new GetNovelsQuery());

        return Ok(mapper.ToResponses(novel));
    }

    [HttpPatch("{novelId:guid}")]
    public async Task<ActionResult<NovelResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromBody] PatchNovelRequest request
    )
    {
        var command = patchNovelCommandMapper.ToCommand(request, novelId);
        
        var novel = await mediator.Send(command);

        return Ok(mapper.ToResponse(novel));
    }

    [HttpDelete("{novelId:guid}")]
    public async Task<ActionResult<NovelResponse>> Delete(
        [FromRoute] Guid novelId
    )
    {
        await mediator.Send(new DeleteNovelCommand(novelId));

        return NoContent();
    }
}