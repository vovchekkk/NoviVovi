using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Characters.CommandMappers;
using NoviVovi.Api.Characters.Mappers;
using NoviVovi.Api.Characters.Requests;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Features.Delete;
using NoviVovi.Application.Characters.Features.Get;

namespace NoviVovi.Api.Characters.Controllers;

[ApiController]
[Tags("Characters")]
[Route("api/novels/{novelId:guid}/characters")]
public class CharactersController(
    IMediator mediator,
    CharacterCommandMapper commandMapper,
    CharacterResponseMapper mapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CharacterResponse>> Add(
        [FromRoute] Guid novelId,
        [FromBody] AddCharacterRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId);

        var character = await mediator.Send(command);

        return Ok(mapper.ToResponse(character));
    }

    [HttpGet("{characterId:guid}")]
    public async Task<ActionResult<CharacterResponse>> Get(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId
    )
    {
        var character = await mediator.Send(new GetCharacterQuery(novelId, characterId));

        return Ok(mapper.ToResponse(character));
    }

    [HttpGet]
    public async Task<ActionResult<CharacterResponse>> Get(
        [FromRoute] Guid novelId
    )
    {
        var character = await mediator.Send(new GetCharactersQuery(novelId));

        return Ok(mapper.ToResponses(character));
    }

    [HttpPatch("{characterId:guid}")]
    public async Task<ActionResult<CharacterResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId,
        PatchCharacterRequest request
    )
    {
        var command = commandMapper.ToCommand(request, novelId, characterId);

        var character = await mediator.Send(command);

        return Ok(mapper.ToResponse(character));
    }

    [HttpDelete("{characterId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId
    )
    {
        await mediator.Send(new DeleteCharacterCommand(novelId, characterId));

        return NoContent();
    }
}