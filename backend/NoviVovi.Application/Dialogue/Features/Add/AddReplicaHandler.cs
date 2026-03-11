using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Dialogue.Features.Add;

public class AddReplicaHandler(
    ILabelRepository labelRepository,
    NovelMapper mapper
)
{
    public async Task<ReplicaSnapshot> Handle(AddReplicaCommand command)
    {
        throw new NotImplementedException();
    }
}