namespace NoviVovi.Infrastructure.DatabaseObjects.Characters;

public class CharacterDbO
{
    public Guid Id { get; set; }
    public Guid NovelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameColor { get; set; } = string.Empty; // HEX без #
    public string? Description { get; set; }
    
    public List<CharacterStateDbO> States { get; set; } = new();
}