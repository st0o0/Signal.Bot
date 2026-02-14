namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to retrieve a list of all attachment IDs stored by the bot.
/// </summary>
public record GetAttachmentsRequest() : RequestBase<List<string>?>("v1/attachments", HttpMethod.Get);