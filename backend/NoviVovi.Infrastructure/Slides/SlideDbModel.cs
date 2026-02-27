namespace NoviVovi.Infrastructure.Slides;

public class SlideDbModel
{
    public int Number { get; set; }
    public string Text { get; set; } = null!;
    public Guid NovelId { get; set; }
}