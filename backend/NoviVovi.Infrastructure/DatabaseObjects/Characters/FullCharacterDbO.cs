namespace NoviVovi.Infrastructure.DatabaseObjects.Characters;

public class FullCharacterDbO
{
    public CharacterDbO Character { get; set; } = new();
    public List<CharacterStateDbO> States { get; set; } = new();
}