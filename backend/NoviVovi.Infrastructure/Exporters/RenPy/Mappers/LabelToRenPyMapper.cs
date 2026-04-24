using NoviVovi.Infrastructure.Exporters.RenPy.Generators;
using NoviVovi.Infrastructure.Exporters.RenPy.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Mappers;

/// <summary>
/// Maps Domain Label to RenPy label with statements.
/// </summary>
public class LabelToRenPyMapper(
    RenPyIdentifierGenerator idGenerator,
    StepToRenPyMapper stepMapper
)
{
    public RenPyLabel Map(Domain.Labels.Label label)
    {
        return new RenPyLabel
        {
            Identifier = idGenerator.GenerateForLabel(label.Id),
            Statements = label.Steps.Select(stepMapper.Map).ToList()
        };
    }
}
