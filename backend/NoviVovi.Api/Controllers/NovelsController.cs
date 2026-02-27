using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Contracts;
using NoviVovi.Api.Contracts.Novels;
using NoviVovi.Application.Novels.Create;
using NoviVovi.Application.Novels.Get;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/novels")]
public class NovelsController : ControllerBase
{
    private readonly CreateNovelHandler _createHandler;
    private readonly GetNovelHandler _getHandler;

    public NovelsController(CreateNovelHandler create, GetNovelHandler get)
    {
        _createHandler = create;
        _getHandler = get;
    }

    [HttpPost]
    public async Task<ActionResult<NovelResponse>> Create(CreateNovelRequest request)
    {
        var novel = await _createHandler.Handle(
            new CreateNovelCommand(request.Title)
        );

        return Ok(novel.ToResponse()); // маппинг Domain/DTO → NovelResponse
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NovelResponse>> Get(Guid id)
    {
        var novel = await _getHandler.Handle(new GetNovelQuery(id));
        if (novel == null) return NotFound();

        return Ok(novel.ToResponse());
    }
}