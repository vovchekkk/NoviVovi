using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;

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