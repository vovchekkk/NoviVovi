using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class StepDbO
{
    public Guid Id { get; set; }
    public Guid LabelId { get; set; }
    public Guid? ReplicaId { get; set; }
    public Guid? MenuId { get; set; }
    public Guid? BgId { get; set; }
    public Guid? NextLabelId { get; set; }
    public int StepOrder { get; set; }
    public string? StepType { get; set; }
    
    public ReplicaDbO? Replica { get; set; }
    public MenuDbO? Menu { get; set; }
    public LabelDbO? NextLabel { get; set; }
    public BackgroundDbO? Background { get; set; }
    public List<StepCharacterDbO> StepCharacters { get; set; } = new();
}