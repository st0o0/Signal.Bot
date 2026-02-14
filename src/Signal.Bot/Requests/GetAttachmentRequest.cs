namespace Signal.Bot.Requests;

/// <summary>
/// Represents a request to download a specific attachment by its unique identifier.
/// </summary>
/// <param name="AttachmentId">The unique identifier of the attachment to retrieve.</param>
public record GetAttachmentRequest(string AttachmentId) : RequestBase($"v1/attachments/{AttachmentId}", HttpMethod.Get);