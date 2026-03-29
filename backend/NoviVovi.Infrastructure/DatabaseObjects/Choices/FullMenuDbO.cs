namespace NoviVovi.Infrastructure.DatabaseObjects.Choices;

public class FullMenuDbO
{
    public MenuDbO Menu { get; set; } = new();
    public List<ChoiceDbO> Choices { get; set; } = new();
}