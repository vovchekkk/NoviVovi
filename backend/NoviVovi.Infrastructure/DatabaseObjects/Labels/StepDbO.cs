using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Enums;
using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Labels;

public class StepDbO
{
    public Guid Id { get; set; }
    public Guid LabelId { get; set; }
    public Guid? ReplicaId { get; set; }
    public Guid? MenuId { get; set; }
    public Guid? BackgroundId { get; set; }
    public Guid? CharacterId { get; set; }
    public Guid? HideCharacterId { get; set; }
    public Guid? NextLabelId { get; set; }
    public int StepOrder { get; set; }
    public string? StepType { get; set; }
    
    

    public StepType Type{get;set;}
    public ReplicaDbO? Replica { get; set; }
    public MenuDbO? Menu { get; set; }
    public LabelDbO? NextLabel { get; set; }
    
    // For ShowCharacterStep - full character with state and transform
    public StepCharacterDbO? Character { get; set; }
    
    // For HideCharacterStep - just the character
    public CharacterDbO? HideCharacter { get; set; }
    
    public BackgroundDbO? Background { get; set; }
}