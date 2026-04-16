using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Images.CommandMappers;
using NoviVovi.Api.Images.Mappers;
using NoviVovi.Api.Images.Requests;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Features.ConfirmUpload;
using NoviVovi.Application.Images.Features.Delete;
using NoviVovi.Application.Images.Features.Get;

namespace NoviVovi.Api.Images.Controllers;

[ApiController]
[Tags("Images")]
[Route("api/images")]
public class ImagesController(
    IMediator mediator,
    InitiateUploadImageCommandMapper initiateUploadCommandMapper,
    PatchImageCommandMapper patchCommandMapper,
    ImageResponseMapper mapper,
    UploadInfoImageResponseMapper uploadInfoImageMapper
) : ControllerBase
{
    /// <summary>
    /// Получить детали конкретного изображения.
    /// </summary>
    [HttpGet("{imageId:guid}", Name = "GetImageById")]
    public async Task<ActionResult<ImageResponse>> Get(
        [FromRoute] Guid imageId
    )
    {
        var imageDto = await mediator.Send(new GetImageQuery(imageId));
        return Ok(mapper.ToResponse(imageDto));
    }

    /// <summary>
    /// Шаг 1: Инициировать загрузку. Получить временную ссылку (Presigned URL) для S3.
    /// Фронтенд должен отправить PUT запрос с файлом по полученному адресу.
    /// </summary>
    [HttpPost("upload-url")]
    public async Task<ActionResult<UploadInfoImageResponse>> InitiateUpload(
        [FromBody] InitiateUploadImageRequest request
    )
    {
        var command = initiateUploadCommandMapper.ToCommand(request);
        var uploadInfo = await mediator.Send(command);

        // Мапим техническую инфу (ID и URL на загрузку)
        return Ok(uploadInfoImageMapper.ToResponse(uploadInfo));
    }

    /// <summary>
    /// Шаг 2: Подтвердить успешную загрузку в облако. 
    /// После этого изображение станет доступно для использования (статус Active).
    /// </summary>
    [HttpPost("{imageId:guid}/confirm")]
    public async Task<ActionResult<ImageResponse>> ConfirmUpload(
        [FromRoute] Guid imageId
    )
    {
        var imageDto = await mediator.Send(new ConfirmUploadImageCommand(imageId));
        return Ok(mapper.ToResponse(imageDto));
    }

    /// <summary>
    /// Обновить метаданные изображения (имя, описание).
    /// </summary>
    [HttpPatch("{imageId:guid}")]
    public async Task<ActionResult<ImageResponse>> Patch(
        [FromRoute] Guid imageId,
        [FromBody] PatchImageRequest request
    )
    {
        var command = patchCommandMapper.ToCommand(request, imageId);
        var imageDto = await mediator.Send(command);
        return Ok(mapper.ToResponse(imageDto));
    }

    /// <summary>
    /// Удалить изображение из БД и из облачного хранилища.
    /// </summary>
    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid imageId
    )
    {
        await mediator.Send(new DeleteImageCommand(imageId));
        return NoContent();
    }
}