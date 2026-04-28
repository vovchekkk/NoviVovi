using System.Text.Json;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Tests.Serialization;

public class AspNetSerializationTest
{
    [Fact]
    public void TestAspNetCoreSerialization()
    {
        // Arrange - simulate what ASP.NET Core does
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new StepResponseConverter());
        options.Converters.Add(new TransitionResponseConverter());
        
        var targetLabelId = Guid.NewGuid();
        ShowMenuStepResponse concreteResponse = new()
        {
            Id = Guid.NewGuid(),
            Menu = new MenuResponse(
                Choices: new List<ChoiceResponse>
                {
                    new(
                        Text: "Choice 1", 
                        Transition: new ChoiceTransitionResponse { TargetLabelId = targetLabelId }
                    )
                }
            ),
            Transition = new NextStepTransitionResponse()
        };

        // Cast to base type (what controller does)
        StepResponse baseResponse = concreteResponse;

        // Act - serialize as base type (what ASP.NET does)
        var json = JsonSerializer.Serialize(baseResponse, options);
        
        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);

        // Assert
        Assert.Contains("\"type\":", json);
        Assert.Contains("\"show_menu\"", json);
    }
}
