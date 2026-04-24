using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;
using Scriban;
using Scriban.Runtime;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Generates script.rpy content using Scriban templates.
/// Follows Single Responsibility Principle: only handles script generation.
/// Follows Dependency Inversion Principle: depends on abstractions (IEmbeddedResourceLoader, IRenPyStatementRenderer).
/// </summary>
public class RenPyScriptGenerator(
    IEmbeddedResourceLoader resourceLoader,
    IRenPyStatementRenderer statementRenderer
) : IRenPyScriptGenerator
{
    private const string TemplateResourceName = "NoviVovi.Infrastructure.Exporters.RenPy.Templates.script.rpy.sbn";

    public async Task<string> GenerateAsync(RenPyNovel novel, CancellationToken ct = default)
    {
        var templateContent = await resourceLoader.LoadTextResourceAsync(TemplateResourceName, ct);
        var template = Template.Parse(templateContent);

        var scriptObject = new ScriptObject();
        scriptObject.Import("render_statement",
            new Func<object, string>(s => statementRenderer.Render((Core.Statements.Models.RenPyStatement)s)));

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        var model = new
        {
            title = novel.Title,
            characters = novel.Characters.Select(c => new
            {
                variable_name = c.VariableName,
                display_name = c.DisplayName,
                color = c.Color
            }),
            labels = novel.Labels.Select(l => new
            {
                identifier = l.Identifier,
                statements = l.Statements
            }),
            start_label_id = novel.StartLabelId
        };

        return await template.RenderAsync(model, member => member.Name);
    }
}