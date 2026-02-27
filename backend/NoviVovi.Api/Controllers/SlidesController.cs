using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Contracts.Novels.Requests;
using NoviVovi.Api.Contracts.Novels.Responses;
using NoviVovi.Api.Mappers;
using NoviVovi.Application.Novels.AddSlide;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/novels/{novelId}/slides")]
public class SlidesController(AddSlideHandler addHandler) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SlideResponse>> Add(Guid novelId, AddSlideRequest request)
    {
        var slide = await addHandler.Handle(new AddSlideCommand(novelId, request.Number, request.Text));
        return Ok(slide.ToResponse());
    }
}