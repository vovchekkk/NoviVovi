using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Preview;

public class VisualSnapshot
{
    public BackgroundObject? Background { get; private set; }
    public Replica? Replica { get; private set; }
    public Menu.Menu? Menu { get; private set; }
    private readonly Dictionary<Guid, CharacterObject> _characters = new();
    
    public IReadOnlyCollection<CharacterObject> CharactersOnScene => 
        _characters.Values.OrderBy(c => c.Transform.ZIndex).ToList();
    
    public void Apply(Step step)
    {
        switch (step)
        {
            case ShowBackgroundStep s:
                Background = s.BackgroundObject;
                break;
            
            case ShowCharacterStep s:
                _characters[s.CharacterObject.Character.Id] = s.CharacterObject;
                break;

            case HideCharacterStep s:
                _characters.Remove(s.Character.Id);
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