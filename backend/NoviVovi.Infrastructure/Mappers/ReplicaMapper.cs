using NoviVovi.Domain.Dialogue;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ReplicaMapper(CharacterMapper charMapper)
{
    public Replica ToReplica(ReplicaDbO rep)
    {
        return new Replica(rep.Id, charMapper.ToCharacter(rep.Speaker), rep.Text);
    }

    public ReplicaDbO ToDbO(Replica rep, Guid novelId)
    {
        return new ReplicaDbO
        {
            Speaker = charMapper.ToDbO(rep.Speaker, novelId),
            Text = rep.Text,
            Id = rep.Id,
            SpeakerId = rep.Speaker.Id
        };
    }
}