namespace NoviVovi.Application.Novels.AddSlide;

public class AddSlideCommand(Guid novelId, int number, string text)
{
    public Guid NovelId { get; set; } = novelId;
    public int Number { get; set; } = number;
    public string Text { get; set; } = text;
}