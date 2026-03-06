using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Dialogue;

public class Replica : Entity
{
    public Character? Speaker { get; private set; }
    public string Text { get; private set; }

    private Replica(Guid id, Character speaker, string text) : base(id)
    {
        Speaker = speaker;
        Text = text;
    }

    public static Replica Create(Character? speaker, string? text)
    {
        if (speaker is null)
            throw new DomainException("Speaker cannot be null");
        
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Text cannot be empty");

        return new Replica(Guid.NewGuid(), speaker, text);
    }

    public static Replica Rehydrate(Guid id, Character speaker, string text)
        => new Replica(id, speaker, text);
}