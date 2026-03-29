using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Novels.Mappers;
using NoviVovi.Api.Novels.Requests.Create;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Features.Create;
using NoviVovi.Application.Novels.Features.Get;

namespace NoviVovi.Api.Novels.Controllers;

[ApiController]
[Route("api/novels")]
public class NovelsController(
    IMediator mediator,
    NovelResponseMapper novelMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create([FromBody] CreateNovelRequest request)
    {
        var novel = await mediator.Send(new CreateNovelCommand(request.Title, request.StartLabelId));
        return Ok(novelMapper.ToResponse(novel));
    }

    [HttpGet("{novelId:guid}")]
    public async Task<ActionResult<NovelResponse>> Get([FromRoute] Guid novelId)
    {
        var novel = await mediator.Send(new GetNovelQuery(novelId));
        return Ok(novelMapper.ToResponse(novel));
    }
}