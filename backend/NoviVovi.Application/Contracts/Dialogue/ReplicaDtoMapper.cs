using NoviVovi.Domain.Dialogue;

namespace NoviVovi.Application.Contracts.Dialogue;

public static class ReplicaDtoMapper
{
    public static ReplicaSnapshot ToDto(this Replica replica)
    {
        return new ReplicaSnapshot
        {
            Id = replica.Id,
            Text = replica.Text
        };
    }
}