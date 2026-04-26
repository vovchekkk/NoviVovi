namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

/// <summary>
/// Generates unique Python-compatible identifiers for Ren'Py export.
/// Uses GUID-based naming to guarantee uniqueness without collisions.
/// </summary>
public class RenPyIdentifierGenerator
{
    private readonly Dictionary<Guid, string> _cache = new();

    /// <summary>
    /// Generates a unique identifier for a Label.
    /// Format: label_{guid} (e.g., label_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6)
    /// </summary>
    public string GenerateForLabel(Guid labelId)
    {
        if (_cache.TryGetValue(labelId, out var cached))
            return cached;

        var identifier = $"label_{labelId:N}"; // N = no hyphens, 32 chars
        _cache[labelId] = identifier;
        return identifier;
    }

    /// <summary>
    /// Generates a unique identifier for a Character.
    /// Format: char_{guid} (e.g., char_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6)
    /// </summary>
    public string GenerateForCharacter(Guid characterId)
    {
        if (_cache.TryGetValue(characterId, out var cached))
            return cached;

        var identifier = $"char_{characterId:N}";
        _cache[characterId] = identifier;
        return identifier;
    }
    
    public string GenerateForCharacterState(Guid stateId)
    {
        if (_cache.TryGetValue(stateId, out var cached))
            return cached;

        var identifier = $"state_{stateId:N}";
        _cache[stateId] = identifier;
        return identifier;
    }

    /// <summary>
    /// Generates a unique identifier for an Image (background).
    /// Format: bg_{guid} (e.g., bg_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6)
    /// </summary>
    public string GenerateForImage(Guid imageId)
    {
        if (_cache.TryGetValue(imageId, out var cached))
            return cached;

        var identifier = $"bg_{imageId:N}";
        _cache[imageId] = identifier;
        return identifier;
    }
}
