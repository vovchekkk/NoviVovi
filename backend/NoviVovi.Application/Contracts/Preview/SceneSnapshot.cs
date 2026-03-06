using NoviVovi.Application.Preview;

namespace NoviVovi.Application.Contracts.Preview;

public class SceneSnapshot
{
    public Guid SessionId { get; set; }

    public string? Background { get; set; }

    public List<CharacterSnapshot> Characters { get; set; }

    public string? Text { get; set; }

    public List<ChoiceSnapshot>? Choices { get; set; }

    public static SceneSnapshot From(ScenePlayer player, Guid sessionId)
    {
        var state = player.State;

        return new SceneSnapshot
        {
            SessionId = sessionId,
            Background = state.Background?.Image.Path,
            Characters = state.Characters
                .Select(c => new CharacterSnapshot(c.Value))
                .ToList(),
            Text = state.Replica?.Text,
            Choices = state.Menu?.Choices
                .Select(c => new ChoiceSnapshot(c))
                .ToList()
        };
    }
}