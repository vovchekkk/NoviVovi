using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview.Services;

public class PreviewSession
{
    public Guid Id { get; }

    public SceneState State { get; }

    public ScenePlayer Player { get; }

    public PreviewSession(Novel novel)
    {
        Id = Guid.NewGuid();
        State = new SceneState();
        Player = new ScenePlayer(State);

        Player.Initialize(novel.StartLabelId);
    }
}