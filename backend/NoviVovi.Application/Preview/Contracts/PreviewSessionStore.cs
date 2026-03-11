using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview.Contracts;

public class PreviewSessionStore
{
    private readonly Dictionary<Guid, PreviewSession> _sessions = new();

    public PreviewSession Create(Novel novel)
    {
        var session = new PreviewSession(novel);

        _sessions[session.Id] = session;

        return session;
    }

    public async Task<PreviewSession?> GetByIdAsync(Guid id)
    {
        return _sessions.GetValueOrDefault(id);
    }
    
    public async Task<PreviewSession> AddAsync(Novel novel)
    {
        var session = new PreviewSession(novel);
        _sessions.Add(session.Id, session);
        return session;
    }

    public async Task RemoveAsync(Guid id)
    {
        _sessions.Remove(id);
    }
}