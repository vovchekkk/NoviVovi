using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Preview.Models;

public class SceneState
{
    public BackgroundObject? Background { get; set; }
    public Replica? Replica { get; set; }
    public Domain.Menu.Menu? Menu { get; set; }
    private readonly Dictionary<Guid, CharacterObject> _characters = new();

    public IReadOnlyDictionary<Guid, CharacterObject> Characters => _characters;

    public void Reset()
    {
        HideBackground();
        HideAllCharacters();
        HideReplica();
        HideMenu();
    }

    public void ShowBackground(ShowBackgroundStep step)
    {
        Background = step.BackgroundObject;
    }
    
    public void HideBackground()
    {
        Background = null;
    }

    public void ShowCharacter(ShowCharacterStep step)
    {
        _characters[step.CharacterObject.Character.Id] = step.CharacterObject;
    }

    public void HideCharacter(HideCharacterStep step)
    {
        _characters.Remove(step.Character.Id);
    }

    public void HideAllCharacters()
    {
        _characters.Clear();
    }

    public void ShowReplica(ShowReplicaStep step)
    {
        Replica = step.Replica;
    }

    public void HideReplica()
    {
        Replica = null;
    }

    public void ShowMenu(ShowMenuStep step)
    {
        Menu = step.Menu;
    }

    public void HideMenu()
    {
        Menu = null;
    }
}