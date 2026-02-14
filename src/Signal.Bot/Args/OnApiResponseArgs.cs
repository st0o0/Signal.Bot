using Signal.Bot.Requests;

namespace Signal.Bot.Args;

/// <summary>
/// Provides event data for API response events, containing the request details and the corresponding HTTP response.
/// </summary>
/// <param name="Request">The <see cref="IRequest"/> that was sent to the Signal Bot API.</param>
/// <param name="RequestMessage">The <see cref="HttpRequestMessage"/> containing the raw HTTP request details.</param>
/// <param name="ResponseMessage">The <see cref="HttpResponseMessage"/> containing the HTTP response received from the API.</param>
public record OnApiResponseArgs(
    IRequest Request,
    HttpRequestMessage RequestMessage,
    HttpResponseMessage ResponseMessage);