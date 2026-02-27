using Microsoft.AspNetCore.Mvc;
using NoviVovi.Api.Contracts;
using NoviVovi.Application.Novels.AddSlide;

namespace NoviVovi.Api.Controllers;

[ApiController]
[Route("api/novels/{novelId}/slides")]
public class SlidesController : ControllerBase
{
    private readonly AddSlideHandler _addHandler;

    public SlidesController(AddSlideHandler addHandler)
    {
        _addHandler = addHandler;
    }

    [HttpPost]
    public async Task<ActionResult<SlideResponse>> Add(Guid novelId, AddSlideRequest request)
    {
        var slide = await _addHandler.Handle(new AddSlideCommand(novelId, request.Number, request.Text));
        return Ok(slide.ToResponse());
    }
}