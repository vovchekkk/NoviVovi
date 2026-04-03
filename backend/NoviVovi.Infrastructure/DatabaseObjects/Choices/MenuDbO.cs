namespace NoviVovi.Infrastructure.DatabaseObjects.Choices;

public class MenuDbO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string? Description { get; set; }
    
    public List<ChoiceDbO> Choices { get; set; } = new();
}