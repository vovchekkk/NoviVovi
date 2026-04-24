using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Application.Novels.Models;

public class NovelGraphBuilder
{
    public NovelGraph Build(Novel novel)
    {
        var nodes = new List<Node>();
        var edges = new List<Edge>();

        foreach (var label in novel.Labels)
        {
            nodes.Add(new Node 
            { 
                Id = label.Id, 
                Name = label.Name
            });
            
            foreach (var step in label.Steps)
            {
                ProcessStep(label.Id, step, edges);
            }
        }

        return new NovelGraph
        {
            Nodes = nodes,
            Edges = edges
        };
    }

    private void ProcessStep(Guid sourceLabelId, Step step, List<Edge> edges)
    {
        if (step is JumpStep jump)
        {
            if (jump.Transition is JumpTransition jumpTransition)
            {
                edges.Add(new JumpEdge 
                { 
                    Id = step.Id,
                    SourceLabelId = sourceLabelId,
                    TargetLabelId = jumpTransition.TargetLabel.Id
                });
            }
        }
        else if (step is ShowMenuStep menuStep)
        {
            foreach (var choice in menuStep.Menu.Choices)
            {
                edges.Add(new ChoiceEdge 
                { 
                    Id = choice.Id,
                    SourceLabelId = sourceLabelId,
                    TargetLabelId = choice.Transition.TargetLabel.Id,
                    Choice = choice,
                    Text = choice.Text
                });
            }
        }
    }
}