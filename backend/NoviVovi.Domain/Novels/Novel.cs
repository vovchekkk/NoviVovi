using NoviVovi.Domain.Slides;

namespace NoviVovi.Domain.Novels;

public class Novel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public List<Slide> Slides { get; set; } = new();
}