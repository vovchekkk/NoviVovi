using System.Text.Json;
using System.Text.Json.Serialization;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Infrastructure;

public class TransitionResponseConverter : JsonConverter<TransitionResponse>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(TransitionResponse).IsAssignableFrom(typeToConvert);
    }

    public override TransitionResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Check if type discriminator exists
        if (!root.TryGetProperty("type", out var typeProperty))
        {
            // No type property - assume NextStepTransition (empty object)
            return new NextStepTransitionResponse();
        }

        var type = typeProperty.GetString();
        
        var tempOptions = new JsonSerializerOptions(options);
        tempOptions.Converters.Clear(); // Remove this converter to avoid recursion
        
        return type switch
        {
            "next_step" => new NextStepTransitionResponse(),
            "jump" => JsonSerializer.Deserialize<JumpTransitionResponse>(root.GetRawText(), tempOptions),
            "choice" => JsonSerializer.Deserialize<ChoiceTransitionResponse>(root.GetRawText(), tempOptions),
            _ => throw new JsonException($"Unknown transition type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, TransitionResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        // Always write type discriminator
        writer.WriteString("type", value switch
        {
            NextStepTransitionResponse => "next_step",
            JumpTransitionResponse => "jump",
            ChoiceTransitionResponse => "choice",
            _ => throw new JsonException($"Unknown transition type: {value.GetType().Name}")
        });

        // Write additional properties for non-empty types
        if (value is JumpTransitionResponse jump)
        {
            writer.WriteString("targetLabelId", jump.TargetLabelId);
        }
        else if (value is ChoiceTransitionResponse choice)
        {
            writer.WriteString("targetLabelId", choice.TargetLabelId);
        }
        
        writer.WriteEndObject();
    }
}
