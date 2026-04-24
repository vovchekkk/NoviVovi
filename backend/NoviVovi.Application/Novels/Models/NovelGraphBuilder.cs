using NoviVovi.Application.Menu.Mappers;
using NoviVovi.Application.Novels.Models.Edges;
using NoviVovi.Application.Novels.Models.Nodes;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Novels.Models;

public class NovelGraphBuilder
{
    public NovelGraph Build(Novel novel)
    {
        var nodes = new List<Node>();
        var edges = new List<Edge>();

        foreach (var label in novel.Labels)
        {
            var step = DetermineLabelTypeStep(label);

            switch (step)
            {
                case ShowMenuStep menuStep:
                    BuildMenuNode(label, menuStep, nodes, edges);
                    break;
                case JumpStep jumpStep:
                    BuildJumpNode(label, jumpStep, nodes, edges);
                    break;
                default:
                    BuildDefaultNode(label, nodes);
                    break;
            }
        }

        return new NovelGraph
        {
            Nodes = nodes,
            Edges = edges
        };
    }

    private Step? DetermineLabelTypeStep(Label label)
    {
        foreach (var step in label.Steps)
        {
            if (step is ShowMenuStep or JumpStep)
                return step;
        }

        return null;
    }

    private void BuildMenuNode(Label label, ShowMenuStep menuStep, List<Node> nodes, List<Edge> edges)
    {
        nodes.Add(new MenuNode
        {
            LabelId = label.Id,
            LabelName = label.Name,
            Choices = menuStep.Menu.Choices
        });

        foreach (var choice in menuStep.Menu.Choices)
        {
            edges.Add(new ChoiceEdge
            {
                StepId = menuStep.Id,
                SourceLabelId = label.Id,
                TargetLabelId = choice.Transition.TargetLabel.Id,
                SourceChoiceId = choice.Id
            });
        }
    }

    private void BuildJumpNode(Label label, JumpStep jumpStep, List<Node> nodes, List<Edge> edges)
    {
        nodes.Add(new JumpNode
        {
            LabelId = label.Id,
            LabelName = label.Name
        });
        
        edges.Add(new JumpEdge
        {
            StepId = jumpStep.Id,
            SourceLabelId = label.Id,
            TargetLabelId = jumpStep.Transition.TargetLabel.Id
        });
    }

    private void BuildDefaultNode(Label label, List<Node> nodes)
    {
        nodes.Add(new Node
        {
            LabelId = label.Id,
            LabelName = label.Name
        });
    }
}