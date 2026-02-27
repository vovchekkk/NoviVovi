namespace NoviVovi.Application.Novels.DTO;

public class NovelDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public List<SlideDto> Slides { get; init; } = new();
}