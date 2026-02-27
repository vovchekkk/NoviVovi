using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.DTO;

public static class NovelDtoMapper
{
    public static NovelDto ToDto(this Novel novel)
    {
        return new NovelDto
        {
            Id = novel.Id,
            Title = novel.Title,
            Slides = novel.Slides.Select(s => new SlideDto
            {
                Number = s.Number,
                Text = s.Text
            }).ToList()
        };
    }
}