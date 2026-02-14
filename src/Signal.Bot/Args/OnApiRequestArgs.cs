using Signal.Bot.Requests;

namespace Signal.Bot.Args;

/// <summary>
/// Provides event data for API request events, containing the request details before it is sent to the Signal Bot API.
/// </summary>
/// <param name="Request">The <see cref="IRequest"/> that will be sent to the Signal Bot API.</param>
/// <param name="RequestMessage">The <see cref="HttpRequestMessage"/> containing the raw HTTP request details.</param>
public record OnApiRequestArgs(
    IRequest Request,
    HttpRequestMessage RequestMessage);