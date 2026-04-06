using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Images.CommandMappers;
using NoviVovi.Api.Images.Mappers;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Features.Delete;
using NoviVovi.Application.Images.Features.Get;

namespace NoviVovi.Api.Images.Controllers;

[ApiController]
[Tags("Images")]
[Route("api/images")]
public class ImagesController(
    IMediator mediator,
    UploadImageCommandMapper uploadCommandMapper,
    PatchImageCommandMapper patchCommandMapper,
    ImageResponseMapper mapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ImageResponse>> Upload(
        [FromBody] UploadImageRequest request
    )
    {
        var command = uploadCommandMapper.ToCommand(request);

        var image = await mediator.Send(command);

        return Ok(mapper.ToResponse(image));
    }
    
    [HttpGet("{imageId:guid}")]
    public async Task<ActionResult<ImageResponse>> Get(
        [FromRoute] Guid imageId
    )
    {
        var image = await mediator.Send(new GetImageQuery(imageId));

        return Ok(mapper.ToResponse(image));
    }
    
    [HttpPatch("{imageId:guid}")]
    public async Task<ActionResult<ImageResponse>> Patch(
        [FromRoute] Guid imageId,
        [FromBody] PatchImageRequest request
    )
    {
        var command = patchCommandMapper.ToCommand(request, imageId);

        var image = await mediator.Send(command);

        return Ok(mapper.ToResponse(image));
    }
    
    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid imageId
    )
    {
        await mediator.Send(new DeleteImageCommand(imageId));

        return NoContent();
    }
}