using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.DTO;
using NoviVovi.Domain.Slides;

namespace NoviVovi.Application.Novels.AddSlide;

public class AddSlideHandler(INovelRepository repo)
{
    public async Task<SlideDto> Handle(AddSlideCommand cmd)
    {
        var slide = new Slide(cmd.Number, cmd.Text);

        await repo.Save(slide);

        return slide.ToDto();
    }
}