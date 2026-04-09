using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Preview;

public class VisualSnapshot
{
    public BackgroundObject? Background { get; private set; }
    public Replica? Replica { get; private set; }
    public Menu.Menu? Menu { get; private set; }
    private readonly List<CharacterObject> _characters = new();
    
    public IReadOnlyCollection<CharacterObject> CharactersOnScene => _characters.AsReadOnly();
    
    public void Apply(Step step)
    {
        switch (step)
        {
            case ShowBackgroundStep s:
                Background = s.BackgroundObject;
                break;
            case ShowCharacterStep s:
                _characters.Add(s.CharacterObject);
                break;
            case HideCharacterStep s:
                _characters.Remove(s.CharacterObject);
                break;
            case ShowReplicaStep s:
                Replica = s.Replica;
                Menu = null;
                break;
            case ShowMenuStep s:
                Menu = s.Menu;
                Replica = null;
                break;
        }
    }
}