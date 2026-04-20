using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace api;

public class GreetFunction
{
    private readonly ILogger<GreetFunction> _logger;

    public GreetFunction(ILogger<GreetFunction> logger)
    {
        _logger = logger;
    }

    [Function("Greet")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greet")] HttpRequest req)
    {
        _logger.LogInformation("Greet function triggered.");

        string name = req.Query["name"].FirstOrDefault() ?? "World";
        string prompt = req.Query["prompt"].FirstOrDefault() ?? "";

        var response = string.IsNullOrEmpty(prompt)
            ? $"Hello, {name}! Welcome to the AI Chat Assistant. How can I help you today?"
            : $"Hello, {name}! You asked: \"{prompt}\". This is a placeholder response — the AI backend integration is coming soon!";

        return new OkObjectResult(new { message = response });
    }
}
