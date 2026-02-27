using NoviVovi.Domain.Slides;

namespace NoviVovi.Application.Novels.DTO;

public static class SlideDtoMapper
{
    public static SlideDto ToDto(this Slide slide)
    {
        return new SlideDto
        {
            Number = slide.Number,
            Text = slide.Text
        };
    }
}