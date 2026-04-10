using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Characters;

public class StepCharacterDbO
{
    public Guid Id { get; set; }
    public Guid? TransformId { get; set; }
    public Guid CharacterStateId { get; set; }
    public TransformDbO? Transform { get; set; }
    public CharacterStateDbO? CharacterState { get; set; }
}