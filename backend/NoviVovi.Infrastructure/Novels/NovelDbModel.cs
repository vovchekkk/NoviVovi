using NoviVovi.Infrastructure.Slides;

namespace NoviVovi.Infrastructure.Novels;

public class NovelDbModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<SlideDbModel> Slides { get; set; } = new();
}