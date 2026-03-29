namespace NoviVovi.Infrastructure.DatabaseObjects.Choices;

public class ChoiceDbO
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Guid NextLabelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}