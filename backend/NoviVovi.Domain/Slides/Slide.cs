using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Slides;

public class Slide(int number, string text)
{
    public int Number { get; set; } = number;
    public string Text { get; set; } = text;
}