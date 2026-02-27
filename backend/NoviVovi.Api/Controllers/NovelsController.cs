using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Contracts.Novels.Requests;
using NoviVovi.Api.Contracts.Novels.Responses;
using NoviVovi.Api.Mappers;
using NoviVovi.Application.Novels.Create;
using NoviVovi.Application.Novels.Get;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/novels")]
public class NovelsController(CreateNovelHandler create, GetNovelHandler get) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create(CreateNovelRequest request)
    {
        var novel = await create.Handle(
            new CreateNovelCommand(request.Title)
        );

        return Ok(novel.ToResponse()); // маппинг Domain/DTO → NovelResponse
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NovelResponse>> Get(Guid id)
    {
        var novel = await get.Handle(new GetNovelQuery(id));
        if (novel == null) return NotFound();

        return Ok(novel.ToResponse());
    }
}