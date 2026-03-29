using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Novels.Mappers;
using NoviVovi.Api.Novels.Requests.Create;
using NoviVovi.Api.Novels.Requests.Get;
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Features.Create;
using NoviVovi.Application.Novels.Features.Get;

namespace NoviVovi.Api.Novels.Controllers;

[ApiController]
[Route("api/novels")]
public class NovelsController(NovelResponseMapper novelMapper, CreateNovelHandler create, GetNovelHandler get) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create(CreateNovelRequest request)
    {
        var novel = await create.Handle(
            new CreateNovelCommand(request.Title, request.StartLabelId)
        );

        return Ok(novelMapper.ToResponse(novel));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NovelResponse>> Get(GetNovelRequest request)
    {
        var novel = await get.Handle(new GetNovelQuery(request.NovelId));
        if (novel == null) return NotFound();

        return Ok(novelMapper.ToResponse(novel));
    }
}