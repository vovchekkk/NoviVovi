using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using NoviVovi.Api.Steps.Responses;

namespace NoviVovi.Api.Infrastructure;

public static class ControllerExtensions
{
    private static readonly JsonSerializerOptions StepJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(),
            new TransitionResponseConverter(),
            new StepResponseConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public static JsonResult ToStepJsonResult(this ControllerBase controller, StepResponse response)
    {
        return new JsonResult(response, StepJsonOptions);
    }
    
    public static JsonResult ToStepJsonResult(this ControllerBase controller, IEnumerable<StepResponse> responses)
    {
        return new JsonResult(responses, StepJsonOptions);
    }
}
