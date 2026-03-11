using System.Collections.Concurrent;
using NoviVovi.Application.Preview.Models;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview.Services;

public class PreviewSessionStore
{
    private readonly ConcurrentDictionary<Guid, PreviewSession> _sessions = new();

    public async Task<PreviewSession> CreateAsync(Novel novel)
    {
        var session = new PreviewSession(novel);

        _sessions[session.Id] = session;

        return await Task.FromResult(session);
    }

    public async Task<PreviewSession?> GetByIdAsync(Guid id)
    {
        _sessions.TryGetValue(id, out var session);
        return await Task.FromResult(session);
    }
    
    public async Task SaveAsync(PreviewSession session)
    {
        _sessions[session.Id] = session;
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id)
    {
        _sessions.TryRemove(id, out _);
        await Task.CompletedTask;
    }
}