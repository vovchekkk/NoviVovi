using NoviVovi.Domain.Common;
using NoviVovi.Domain.Replicas;

namespace NoviVovi.Domain.Slides;

public class Slide(int number, string text)
{
    private readonly List<Replica> _replicas = new();
    public int Number { get; set; } = number;
    public string Text { get; set; } = text;

    public IReadOnlyList<Replica> Replicas => _replicas.AsReadOnly();

    public void AddReplica(Replica replica)
    {
        if (_replicas.Any(s => s.Id == replica.Id))
            throw new DomainException($"Slide {replica.Id} already exists");
        _replicas.Add(replica);
    }

    public void RemoveReplica(Guid replicaId)
    {
        var replica = _replicas.FirstOrDefault(r => r.Id == replicaId);
        if (replica == null)
            throw new DomainException($"Replica {replicaId} not found");
        _replicas.Remove(replica);
    }
}