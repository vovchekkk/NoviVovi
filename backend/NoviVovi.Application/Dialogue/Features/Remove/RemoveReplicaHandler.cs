using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Dialogue.Features.Remove;

public class RemoveReplicaHandler(
    ILabelRepository labelRepository,
    NovelMapper mapper
)
{
    public async Task<ReplicaSnapshot> Handle(RemoveReplicaCommand command)
    {
        throw new NotImplementedException();
    }
}