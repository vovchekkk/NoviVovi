using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Replicas;

public class Replica
{
    public Guid Id { get; private set; }
    public string Text { get; private set; }

    public Replica(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Replica text cannot be empty");
        Text = text;
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Replica text cannot be empty");
        Text = text;
    }
}