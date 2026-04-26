using NoviVovi.Domain.Dialogue;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ReplicaMapper(
    CharacterMapper charMapper
)
{
    public Replica ToDomain(ReplicaDbO rep)
    {
        if (rep is { Speaker: not null, Text: not null })
            return new Replica(rep.Id, charMapper.ToDomain(rep.Speaker), rep.Text);
        throw new ArgumentException("Incorrect replica");
    }

    public ReplicaDbO ToDbO(Replica rep)
    {
        return new ReplicaDbO
        {
            Speaker = charMapper.ToDbO(rep.Speaker),
            Text = rep.Text,
            Id = rep.Id,
            SpeakerId = rep.Speaker.Id
        };
    }
}