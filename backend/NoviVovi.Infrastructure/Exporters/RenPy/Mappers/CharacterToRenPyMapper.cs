using NoviVovi.Infrastructure.Exporters.RenPy.Generators;
using NoviVovi.Infrastructure.Exporters.RenPy.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Mappers;

/// <summary>
/// Maps Domain Character to RenPy character definition.
/// </summary>
public class CharacterToRenPyMapper(
    RenPyIdentifierGenerator idGenerator
)
{
    public RenPyCharacter Map(Domain.Characters.Character character)
    {
        return new RenPyCharacter
        {
            VariableName = idGenerator.GenerateForCharacter(character.Id),
            DisplayName = character.Name,
            Color = character.NameColor.Value
        };
    }
}