using NoviVovi.Domain.Replicas;

namespace NoviVovi.Application.Novels.DTO;

public static class ReplicaDtoMapper
{
    public static ReplicaDto ToDto(this Replica replica)
    {
        return new ReplicaDto
        {
            Id = replica.Id,
            Text = replica.Text
        };
    }
}