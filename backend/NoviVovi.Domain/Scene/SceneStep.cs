using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.StoryFlow;

namespace NoviVovi.Domain.Scene;

public class SceneStep : Entity
{
    public BackgroundObject? Background { get; private set; }
    private readonly List<CharacterObject> _characters = new();

    public IReadOnlyList<CharacterObject> Characters => _characters;

    public Character? Speaker { get; private set; }
    public Replica? Replica { get; private set; }
    public Transition Transition { get; private set; }

    private SceneStep(Guid id) : base(id)
    {
    }

    public static SceneStep Create() => new(Guid.NewGuid());

    public static SceneStep Rehydrate(
        Guid id,
        BackgroundObject background,
        IEnumerable<CharacterObject> characterObjects,
        Character speaker,
        Replica replica,
        Transition transition)
    {
        var sceneStep = new SceneStep(id);
        
        sceneStep.SetBackground(background);
        foreach (var characterObject in characterObjects)
            sceneStep.AddCharacter(characterObject);
        sceneStep.SetDialogue(speaker, replica);
        sceneStep.SetTransition(transition);
        
        return sceneStep;
    }

    public void SetBackground(BackgroundObject background)
    {
        Background = background;
    }

    public void AddCharacter(CharacterObject characterObject)
    {
        if (_characters.Any(item => item.Id == characterObject.Id))
            throw new DomainException($"Slide {characterObject.Id} already exists");
        _characters.Add(characterObject);
    }

    public void SetDialogue(Character speaker, Replica replica)
    {
        Speaker = speaker;
        Replica = replica;
    }
    
    public void SetTransition(Transition transition)
    {
        Transition = transition;
    }
}