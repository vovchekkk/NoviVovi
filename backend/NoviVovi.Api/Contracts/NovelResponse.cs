namespace NoviVovi.Api.Contracts;

public class NovelResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<SlideResponse> Slides { get; set; } = new();
}