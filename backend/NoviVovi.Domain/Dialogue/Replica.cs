using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Dialogue;

public class Replica : Entity
{
    public string Text { get; private set; }

    private Replica(Guid id, string text) : base(id)
    {
        Text = text;
    }

    public static Replica Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Text cannot be null");

        return new Replica(Guid.NewGuid(), text);
    }

    public static Replica Rehydrate(Guid id, string text)
        => new Replica(id, text);
}