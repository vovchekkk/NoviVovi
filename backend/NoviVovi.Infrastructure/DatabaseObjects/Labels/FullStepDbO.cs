using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class FullStepDbO
{
    public StepDbO Step { get; set; } = new();
    public ReplicaDbO? Replica { get; set; }
    public FullMenuDbO? Menu { get; set; }
    public BackgroundDbO? Background { get; set; }
    public List<StepCharacterDbO> StepCharacters { get; set; } = new();
}