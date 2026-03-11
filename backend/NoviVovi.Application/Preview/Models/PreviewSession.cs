using NoviVovi.Application.Preview.Services;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview.Models;

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