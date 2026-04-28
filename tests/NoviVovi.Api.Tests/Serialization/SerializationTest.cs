using System.Text.Json;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Api.Dialogue.Responses;

namespace NoviVovi.Api.Tests.Serialization;

public class SerializationTest
{
    [Fact]
    public void TestShowReplicaStepResponseSerialization()
    {
        // Arrange
        var response = new ShowReplicaStepResponse
        {
            Id = Guid.NewGuid(),
            Transition = new NextStepTransitionResponse(),
            Replica = new ReplicaResponse(
                Id: Guid.NewGuid(),
                SpeakerId: Guid.NewGuid(),
                Text: "Hello"
            )
        };

        // Act - serialize as concrete type first
        var json = JsonSerializer.Serialize(response);
        Console.WriteLine("Serialized JSON (concrete type):");
        Console.WriteLine(json);

        // Now serialize as base type
        var jsonBase = JsonSerializer.Serialize<StepResponse>(response);
        Console.WriteLine("\nSerialized JSON (base type):");
        Console.WriteLine(jsonBase);

        // Try to deserialize from base type JSON
        Console.WriteLine("\nAttempting deserialization...");
        var deserialized = JsonSerializer.Deserialize<StepResponse>(jsonBase);
        
        // Assert
        Assert.NotNull(deserialized);
        Assert.IsType<ShowReplicaStepResponse>(deserialized);
        
        var typed = (ShowReplicaStepResponse)deserialized;
        Assert.NotNull(typed.Transition);
        Console.WriteLine($"\nDeserialization successful! Transition type: {typed.Transition.GetType().Name}");
    }
}
