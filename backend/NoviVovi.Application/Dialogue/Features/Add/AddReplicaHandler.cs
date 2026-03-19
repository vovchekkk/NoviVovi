using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Application.Dialogue.Features.Add;

public class AddReplicaHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    ReplicaMapper mapper
)
{
    public async Task<ReplicaSnapshot> Handle(AddReplicaCommand command)
    {
        var novel = await novelRepository.GetByIdAsync(command.NovelId);
        if (novel == null)
            throw new NotFoundException($"Новелла с ID '{command.NovelId}' не найдена");
        
        var speaker = novel.Characters.FirstOrDefault(item => item.Id == command.SpeakerId);
        
        var replica = Replica.Create(speaker, command.Text);

        var step = ShowReplicaStep.Create(replica);
        
        var label = await labelRepository.GetByIdAsync(command.LabelId);
        if (label == null)
            throw new NotFoundException($"Метка с ID '{command.LabelId}' не найдена");
        label.AddStep(step);

        return mapper.ToSnapshot(replica);
    }
}