using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class NovelMapper(
    CharacterMapper characterMapper,
    LabelMapper labelMapper
)
{
    public Novel ToDomain(NovelDbO dbo)
    {
        if (dbo.StartLabel == null)
            throw new ArgumentException("Novel startLabel is null");
        
        var result = new Novel(dbo.Id, dbo.Title, labelMapper.ToDomain(dbo.StartLabel, new MappingContext()));
        foreach (var label in dbo.Labels)
            result.AddLabel(labelMapper.ToDomain(label, new MappingContext()));
        
        foreach (var character in dbo.Characters)
            result.AddCharacter(characterMapper.ToDomain(character));
        return result;
    }

    public NovelDbO ToDbO(Novel domain)
    {
        
        var result = new NovelDbO
        {
            Id = domain.Id,
            StartLabelId = domain.StartLabel?.Id,
            Title = domain.Title,
            IsPublic = true,
            Characters = domain.Characters.Select(c => characterMapper.ToDbO(c, domain.Id)).ToList(),
            Labels = domain.Labels.Select(l => labelMapper.ToDbO(l, new MappingContext())).ToList(),
            StartLabel = domain.StartLabel == null ? null : labelMapper.ToDbO(domain.StartLabel, new MappingContext())
        };
        return result;
    }
}