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
[Tags("CharacterStates")]
[Route("api/novels/{novelId:guid}/characters/{characterId:guid}/states")]
public class CharacterStatesController(
    IMediator mediator,
    AddCharacterStateCommandMapper addCommandMapper,
    PatchCharacterStateCommandMapper patchCommandMapper,
    CharacterStateResponseMapper mapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CharacterStateResponse>> Add(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId,
        [FromBody] AddCharacterStateRequest request
    )
    {
        var command = addCommandMapper.ToCommand(request, novelId, characterId);

        var characterState = await mediator.Send(command);

        return Ok(mapper.ToResponse(characterState));
    }

    [HttpGet("{stateId:guid}")]
    public async Task<ActionResult<CharacterStateResponse>> Get(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId,
        [FromRoute] Guid stateId
    )
    {
        var characterState = await mediator.Send(new GetCharacterStateQuery(novelId, characterId, stateId));

        return Ok(mapper.ToResponse(characterState));
    }

    [HttpGet]
    public async Task<ActionResult<CharacterStateResponse>> Get(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId
    )
    {
        var characterState = await mediator.Send(new GetCharacterStatesQuery(novelId, characterId));

        return Ok(mapper.ToResponses(characterState));
    }

    [HttpPatch("{stateId:guid}")]
    public async Task<ActionResult<CharacterStateResponse>> Patch(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId,
        [FromRoute] Guid stateId,
        PatchCharacterStateRequest request
    )
    {
        var command = patchCommandMapper.ToCommand(request, novelId, characterId, stateId);

        var characterState = await mediator.Send(command);

        return Ok(mapper.ToResponse(characterState));
    }

    [HttpDelete("{stateId:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid novelId,
        [FromRoute] Guid characterId,
        [FromRoute] Guid stateId
    )
    {
        await mediator.Send(new DeleteCharacterStateCommand(novelId, characterId, stateId));

        return Ok();
    }
}