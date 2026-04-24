namespace NoviVovi.Infrastructure.DatabaseObjects.Choices;

public class MenuDbO
{
    public Guid Id { get; set; }
    public List<ChoiceDbO> Choices { get; set; } = new();
}