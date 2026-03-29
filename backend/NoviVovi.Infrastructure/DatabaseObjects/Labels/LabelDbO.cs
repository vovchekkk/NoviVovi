namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class LabelDbO
{
    public Guid Id { get; set; }
    public Guid NovelId { get; set; }
    public string LabelName { get; set; } = string.Empty;
}