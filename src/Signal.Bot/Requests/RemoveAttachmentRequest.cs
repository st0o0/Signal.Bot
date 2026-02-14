namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to delete a specific attachment from the bot's storage.
/// </summary>
/// <param name="AttachmentId">The unique identifier of the attachment to delete.</param>
public record RemoveAttachmentRequest(string AttachmentId)
    : RequestBase($"v1/attachments/{AttachmentId}", HttpMethod.Delete);