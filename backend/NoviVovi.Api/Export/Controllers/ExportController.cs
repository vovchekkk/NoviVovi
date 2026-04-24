using System.IO.Compression;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoviVovi.Application.Export.Features.Export;

namespace NoviVovi.Api.Export.Controllers;

[ApiController]
[Tags("Export")]
[Route("api")]
public class ExportController(
    IMediator mediator
) : ControllerBase
{
    [HttpGet("{novelId:guid}/export/renpy")]
    public async Task<IActionResult> Export(
        [FromRoute] Guid novelId
    )
    {
        var bytes = await mediator.Send(new ExportNovelToRenPyCommand(novelId));
        
        return File(bytes, "application/zip", "project.zip");
    }
}