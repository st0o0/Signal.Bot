namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to check the health status of the Signal Bot API.
/// </summary>
public record GetHealthRequest() : RequestBase($"v1/health", HttpMethod.Get);
