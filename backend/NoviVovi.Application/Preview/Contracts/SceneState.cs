using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Preview.Contracts;

public class SceneState
{
    public BackgroundObject? Background { get; set; }
    public Replica? Replica { get; set; }
    public Menu? Menu { get; set; }
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
        var obj = BackgroundObject.Create(step.Background, step.Transform);
        Background = obj;
    }
    
    public void HideBackground()
    {
        Background = null;
    }

    public void ShowCharacter(ShowCharacterStep step)
    {
        var obj = CharacterObject.Create(step.Character, step.State, step.Transform);
        _characters[step.Character.Id] = obj;
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