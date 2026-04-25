using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;

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
