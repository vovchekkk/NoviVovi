using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview;

public class PreviewSessionStore
{
    private readonly Dictionary<Guid, PreviewSession> _sessions = new();

    public PreviewSession Create(Novel novel)
    {
        var session = new PreviewSession(novel);

        _sessions[session.Id] = session;

        return session;
    }

    public PreviewSession? Get(Guid id)
    {
        return _sessions.GetValueOrDefault(id);
    }

    public void Remove(Guid id)
    {
        _sessions.Remove(id);
    }
}