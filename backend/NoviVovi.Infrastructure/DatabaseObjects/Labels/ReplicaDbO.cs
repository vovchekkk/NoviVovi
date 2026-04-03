using NoviVovi.Infrastructure.DatabaseObjects.Characters;

namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class ReplicaDbO
{
    public Guid Id { get; set; }
    public Guid? SpeakerId { get; set; }
    public string? Text { get; set; }
    
    public CharacterDbO? Speaker { get; set; }
}
