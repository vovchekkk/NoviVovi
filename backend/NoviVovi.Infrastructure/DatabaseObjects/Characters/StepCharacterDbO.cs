using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Characters;

public class StepCharacterDbO
{
    public Guid Id { get; set; }
    public Guid? TransformId { get; set; }
    public Guid CharacterStateId { get; set; }
    
    public CharacterDbO Character { get; set; } //Я очень против такого, но в основной логике оно так.
                                                //Это блин петля в коде, которую и на шею хочется натянуть
    public TransformDbO? Transform { get; set; }
    public CharacterStateDbO? CharacterState { get; set; }
}