namespace NoviVovi.Api.Contracts.Novels.Responses;

public class NovelResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<SlideResponse> Slides { get; set; } = new();
}